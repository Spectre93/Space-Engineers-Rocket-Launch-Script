// The tag on the programmable block that
// you want to send the stage names to
string _PBTag = "PB-IGC";

// The command to send to the programmble
// block to go to the next stage
const string _NextTag = "Next";

// The command to send to the programmble
// block to go to the previous stage
const string _PreviousTag = "Previous";

// The command to send to the programmble
// block to reset to stage 0
const string _ResetTag = "Reset";

// If true, activates a timer with the
// above tag in addition to setting the
// program back to stage 0. Useful for
// setting pistons or rotors to initial
// positions.
bool CallResetTimerBlockOnReset = true;

// Do not edit below this line unless you know what you're doing
String[] stages = {};
int currentStage = 0;

public Program(){
    // Retrieve previous stage from storage if it exists
    if(Storage.Length > 0){
        currentStage = int.Parse(Storage);
        Echo("Stage found in storage: " + currentStage);
    }

    // Retrieve stage names from custom data
    if(Me.CustomData.Length > 0){
        stages = Me.CustomData.Split(',');
        Echo("Custom data read, amount of stages: " + stages.Length);
        if(currentStage > stages.Length){
            Echo("Stage from storage higher than maximum, setting to last stage instead.");
            currentStage = stages.Length;
        }
    }
}

// Save function, called on world save to store the current stage number
public void Save(){
    Storage = currentStage.ToString();
}

public void Main(string argument){
    if(stages.Length == 0){
        Echo("Specify stages in this block's custom data first, seperated by commas.");
        return;
    }

    switch(argument){
        case _NextTag:
            NextStage();
            break;
        case _PreviousTag:
            PreviousStage();
            break;
        case _ResetTag:
            if(CallResetTimerBlockOnReset){
                SendToPB(_ResetTag);
            }
            currentStage = 0;
            break;
    }
}

// Sets the program to the next stage and 
// sends the relevant stage name to be sent to the PB
private void NextStage(){
    if(currentStage < stages.Length){
        SendToPB(stages[currentStage++]);
    }
}

// Sets the program to the previous stage and 
// sends the relevant stage name to be sent to the PB
private void PreviousStage(){
    if(currentStage > 0){
        SendToPB(stages[--currentStage]);
    }
}

// Sends the stage name to the PB
private void SendToPB(string argument){
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("[" + _PBTag + "]", blocks);
    try{
        var progb = blocks[0] as IMyProgrammableBlock;
        progb.TryRun(argument);
        Echo("Running " + argument);
    }catch(Exception e){
        Echo("Programmable block with tag '[" + _PBTag + "]' not found.");
    }
}