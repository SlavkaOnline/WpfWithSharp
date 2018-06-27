namespace Domain.ModBus

open EasyModbus
open System.Threading.Tasks

module Modbus =
    type Client =
        | ConnectedClient of ModbusClient
        | DisconnectedClient of ModbusClient
    
    type Commands = WriteHoldingRegisters of address : int * registers : int []
    
    type Queries = ReadHoldingRegisters of address : int * count : int
    
    let CreateClient (ip : string) (port : int) = DisconnectedClient(new ModbusClient(ip, port))
    
    let Connect client =
        match client with
        | ConnectedClient(client) as c -> Ok c
        | DisconnectedClient(client) -> 
            try 
                client.Connect()
                Ok <| ConnectedClient(client)
            with :? System.Exception as ex -> Error ex.Message
    
    let private DisconectConnectedClient(ConnectedClient cl) =
        cl.Disconnect()
        cl
    
    let Disconnect client =
        match client with
        | ConnectedClient(client) as c -> 
            c
            |> DisconectConnectedClient
            |> DisconnectedClient
        | DisconnectedClient(client) as c -> c
    
    let CommandHandler client command =
        match client with
        | ConnectedClient c -> 
            try 
                match command with
                | WriteHoldingRegisters(adrs, rgs) -> 
                    c.WriteMultipleRegisters(adrs, rgs)
                    |> Ok
            with :? System.Exception as ex -> Error ex.Message
        | DisconnectedClient c -> Error "Disconnected state"
    
    let QueryHandler client query =
        match client with
        | ConnectedClient c -> 
            try 
                match query with
                | ReadHoldingRegisters(adrs, cnt) -> 
                    c.ReadHoldingRegisters(adrs, cnt)
                    |> Ok
            with :? System.Exception as ex -> Error ex.Message
        | DisconnectedClient c -> Error "Disconnected state"
