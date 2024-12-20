using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

//This will only run on the client because it updates in ClientSimulationSystemGroup (which the server does not have)
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
[UpdateBefore(typeof(RpcSystem))]
public partial class ClientLoadGameSystem : SystemBase
{
    private BeginSimulationEntityCommandBufferSystem m_BeginSimEcb;

    protected override void OnCreate()
    {
        //We will be using the BeginSimECB
        m_BeginSimEcb = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

        //Requiring the ReceiveRpcCommandRequestComponent ensures that update is only run when an NCE exists and a SendClientGameRpc has come in
        RequireForUpdate(GetEntityQuery(ComponentType.ReadOnly<SendClientGameRpc>(), ComponentType.ReadOnly<ReceiveRpcCommandRequestComponent>()));
        //This is just here to make sure the Sub Scene is streamed in before the client sets up the level data
        RequireSingletonForUpdate<GameSettingsComponent>();
        //We will make sure we have our ClientDataComponent so we can send the server our player name
        RequireSingletonForUpdate<ClientDataComponent>();
    }

    protected override void OnUpdate()
    {

        //We must declare our local variables before using them within a job (.ForEach)
        var commandBuffer = m_BeginSimEcb.CreateCommandBuffer();
        var rpcFromEntity = GetBufferFromEntity<OutgoingRpcDataStreamBufferComponent>();
        var gameSettingsEntity = GetSingletonEntity<GameSettingsComponent>();
        var getGameSettingsComponentData = GetComponentDataFromEntity<GameSettingsComponent>();
        var clientData = GetSingleton<ClientDataComponent>(); //We will use this to send the player name to server

        Entities
        .ForEach((Entity entity, in SendClientGameRpc request, in ReceiveRpcCommandRequestComponent requestSource) =>
        {
            //This destroys the incoming RPC so the code is only run once
            commandBuffer.DestroyEntity(entity);

            //Check for disconnects before moving forward
            if (!rpcFromEntity.HasComponent(requestSource.SourceConnection))
                return;

            //Set the game size (unnecessary right now but we are including it to show how it is done)
            getGameSettingsComponentData[gameSettingsEntity] = new GameSettingsComponent
            {
                levelWidth = request.levelWidth,
                levelHeight = request.levelHeight,
                levelDepth = request.levelDepth,
                playerForce = request.playerForce,
                bulletVelocity = request.bulletVelocity
            };


            //Here we create a new singleton entity for GameNameComponent
            //We could add this component to the singleton entity that has the GameSettingsComponent
            //but we will keep them separate in case we want to change workflows in the future and don't
            //want these components to be dependent on the same entity
            var gameNameEntity = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(gameNameEntity, new GameNameComponent
            {
                GameName = request.gameName
            });

            //These update the NCE with NetworkStreamInGame (required to start receiving snapshots) and
            //PlayerSpawningStateComponent, which we will use when we spawn players
            commandBuffer.AddComponent(requestSource.SourceConnection, new PlayerSpawningStateComponent());
            commandBuffer.AddComponent(requestSource.SourceConnection, default(NetworkStreamInGame));

            //This tells the server "I loaded the level"
            //First we create an entity called levelReq that will have 2 necessary components
            //Next we add the RPC we want to send (SendServerGameLoadedRpc) and then we add
            //SendRpcCommandRequestComponent with our TargetConnection being the NCE with the server (which will send it to the server)
            var levelReq = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(levelReq, new SendServerGameLoadedRpc());
            commandBuffer.AddComponent(levelReq, new SendRpcCommandRequestComponent { TargetConnection = requestSource.SourceConnection });

            // this tells the server "This is my name and Id" which will be used for player score tracking
            var playerReq = commandBuffer.CreateEntity();
            commandBuffer.AddComponent(playerReq, new SendServerPlayerNameRpc { playerName = clientData.PlayerName });
            commandBuffer.AddComponent(playerReq, new SendRpcCommandRequestComponent { TargetConnection = requestSource.SourceConnection });

        }).Schedule();

        m_BeginSimEcb.AddJobHandleForProducer(Dependency);
    }
}