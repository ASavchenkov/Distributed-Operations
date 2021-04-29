using Godot;
using System;
using System.Collections.Generic;

using MessagePack;

public interface IBufferedRPC
{
    void HandlePacket(byte[] packet);
}

//Singleton that can be used as an alternative to regular RPCs
//for communications that might accidentally occur before a node is created.
public class BufRPCServer : Node
{
    public static BufRPCServer Instance { get; private set;}

    public Dictionary<string, List<byte[]>> Buffer = new Dictionary<string, List<byte[]>>();

    public override void _Ready()
    {
        Instance = this;
    }

    //path needs to be absolute path
    [Remote]
    public void BufRPC( NodePath path, byte[] packet)
    {
        IBufferedRPC target = (IBufferedRPC) GetNodeOrNull(path);

        if(target is null)
        {
            List<byte[]> packets;
            if(Buffer.TryGetValue(path, out packets))
            {
                packets.Add(packet);
            }
            else{
                packets = new List<byte[]>();
                packets.Add(packet);
                Buffer.Add(path, packets);
            }
        }
        else
        {
            target.HandlePacket(packet);
        }
    }

    public void NotifyNewNode(string path)
    {
        List<byte[]> packets;
        if(Buffer.TryGetValue(path, out packets))
        {
            IBufferedRPC n = (IBufferedRPC) GetNode(path);
            foreach( byte[]  p in packets)
            {
                n.HandlePacket(p);
            }
        }
    }
}
