// Customise the tag if you're using this script in different
// places in the same save or if you're on a server
string _broadCastTag = "IGC-TTFS";

// If true, the script will trigger a timer block
// on both the local and the remote grid
bool alsoRunOnLocalGrid = false;

// Do not edit below this line unless you know what you're doing
IMyBroadcastListener _myBroadcastListener;

public Program() { 
	//Register _broadCastTag to be able to receive messages
	_myBroadcastListener=IGC.RegisterBroadcastListener(_broadCastTag);
 	_myBroadcastListener.SetMessageCallback(_broadCastTag); 
}

public void Main(string argument, UpdateType updateSource){
	HandleCommunication(argument, updateSource);
    if(alsoRunOnLocalGrid){
        TriggerTimerFromString(argument);
    }
}

private void HandleCommunication(string argument, UpdateType updateSource){
	// IGC code to send messages
    if ((updateSource & (UpdateType.Trigger | UpdateType.Script)) > 0){ 
        if (argument != ""){
            IGC.SendBroadcastMessage(_broadCastTag, argument);
        }
    }

	// IGC code to receive messages
    if((updateSource & UpdateType.IGC) >0){ 
        while (_myBroadcastListener.HasPendingMessage){
            MyIGCMessage myIGCMessage = _myBroadcastListener.AcceptMessage();
            if(myIGCMessage.Data is string){
                string str = myIGCMessage.Data.ToString();
                TriggerTimerFromString(str);
            }
        }
    }
}

//Triggers a timer block with the given string in the name surrounded by square brackets
private void TriggerTimerFromString(string str){
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("[" + str + "]", blocks);
    try{
        var timer = blocks[0] as IMyTimerBlock;
        timer.Trigger();
    }catch(Exception e){
        Echo("'[" + str + "]' timer block missing.");
    }
}