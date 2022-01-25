string _broadCastTag = "IGC SE-RLS";
IMyBroadcastListener _myBroadcastListener;

//Constructor with update frequency to auto run script
public Program() { 
	Runtime.UpdateFrequency = UpdateFrequency.Update100;

	//Register _broadCastTag to be able to receive messages
	_myBroadcastListener=IGC.RegisterBroadcastListener(_broadCastTag);
 	_myBroadcastListener.SetMessageCallback(_broadCastTag); 
}

public void Main(string argument, UpdateType updateSource){
	HandleCommunication(argument, updateSource);
}

private void HandleCommunication(string argument, UpdateType updateSource){
	// IGC code to send messages
    if ((updateSource & (UpdateType.Trigger | UpdateType.Script)) > 0){ 
        if (argument != ""){
            IGC.SendBroadcastMessage(_broadCastTag, argument);
            Echo("Sending message:\n" + argument);
        }
    }

	// IGC code to receive messages
    if((updateSource & UpdateType.IGC) >0){ 
        while (_myBroadcastListener.HasPendingMessage){
            MyIGCMessage myIGCMessage = _myBroadcastListener.AcceptMessage();

			//Only listen if it matches our tag
            if(myIGCMessage.Tag == _broadCastTag){

            }
        }
    }
}