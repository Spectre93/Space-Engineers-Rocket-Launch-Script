bool TimerBlock2Ran = false;
List<IMyThrust> thrusters;

// Current distance to the ground - only works on planets
double currentAltitude = 0;
float shipMass = 0;
string _controller = "Controller";
IMyShipController controller;
Vector3D gravityVector;
float gravity = 0;

// Calculate total max thrust in Newtons
float maxEffectiveThrust = 0;
float currentEffectiveThrust = 0;

public Program(){
    thrusters = new List<IMyThrust>();
	GridTerminalSystem.GetBlocksOfType<IMyThrust>(thrusters);
    Runtime.UpdateFrequency = UpdateFrequency.Update1;	//10/100

	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("[" + _controller + "]", blocks);
    try{
        controller = blocks[0] as IMyShipController;
    }catch(Exception e){ 
		Echo("No ship controller marked as '[" + _controller + "]' found.");
		Runtime.UpdateFrequency = UpdateFrequency.None;
	}
}

public void Save(){

}



public void Main(string argument, UpdateType updateSource){
    shipMass = controller.CalculateShipMass().PhysicalMass;
	Echo("Mass: " + shipMass.ToString());
	
	controller.TryGetPlanetElevation(MyPlanetElevation.Surface, out currentAltitude);
	Echo("Altitude: " + currentAltitude.ToString());

	double currentAltitudeToSea = 0;
	controller.TryGetPlanetElevation(MyPlanetElevation.Sealevel, out currentAltitudeToSea);
	Echo("AltitudeToSea: " + currentAltitudeToSea.ToString());

	Vector3D pos = controller.GetPosition();
	Vector3D p;

	controller.TryGetPlanetPosition(out p);
	Echo("p = " + (p-pos).Length());

    maxEffectiveThrust = 0;
	foreach (IMyThrust thruster in thrusters){
		//Echo("Name: " + thruster.Name); this is just a number
		//Echo("disp Name: " + thruster.BlockDefinition);
		//Echo("Custom name: " + thruster.CustomName); name that we can give
		//Echo("DefinitionDisplayNameText: " + thruster.DefinitionDisplayNameText);
		if(thruster.DefinitionDisplayNameText.Contains("Atmospheric")){
			if (thruster.Orientation.Forward == Base6Directions.Direction.Down){
				maxEffectiveThrust += thruster.MaxEffectiveThrust;
			}
		}
	}

    currentEffectiveThrust = 0;
	foreach (IMyThrust thruster in thrusters){
		if (thruster.Orientation.Forward == Base6Directions.Direction.Down){
			maxEffectiveThrust += thruster.MaxEffectiveThrust;
			currentEffectiveThrust += thruster.CurrentThrust;
		}
	}

    Echo("Current effective: " + ((int)currentEffectiveThrust).ToString());
	
	gravityVector = controller.GetNaturalGravity();
	gravity = (float)(gravityVector.Length() / 9.81);
	Echo("Gravity (g): " + gravity.ToString());
	
	float fdown = shipMass * gravity;
	Echo("Fdown: " + fdown);
	//float fup = maxEffectiveThrust / (float)gravityVector.Length();
	Echo("Max effective: " + ((int)maxEffectiveThrust).ToString());
	Echo("Fres:" + (maxEffectiveThrust-fdown));
	Echo("a: " + (maxEffectiveThrust/shipMass));
    Echo("Speed: " + controller.GetShipSpeed());
	
	if (!TimerBlock2Ran & (maxEffectiveThrust< fdown)){		
		var timer = GridTerminalSystem.GetBlockWithName("Timer Block 2") as IMyTimerBlock;
		timer.Trigger();
		TimerBlock2Ran = true;
	}	
	
	//p-pos > 21147
	if(currentAltitude>23075){	//21.147
		var hydro = GridTerminalSystem.GetBlockWithName("Large Hydrogen Thruster 7") as IMyThrust;
		hydro.Enabled = false;
	}
}

/*
Todo
test sending tuples
var t = (speed: 4.5, altitude: 3);
Echo($"Speed is {t.speed} and rocket is currently at {t.altitude} meters up);
59400
67200
5800

Distance where the gravity falls off entirely
(67200/x)^7 = 0.05
=> x = 103093

Derivative of acceleration function, equal to -200 yields distance from 103093 to stop thrusters
d/dr(9.81 (67200/r)^7) = -424963821495373669451793352666120192/r^8
-424963821495373669451793352666120192/r^8 = -200 
=> r = 14653
for -110, r = 15790

103000 - 67200 = 35800
35800 - 14653 = 21147


Top Speed = ((GridMass - MinMass) / (MaxMass - MinMass)) * (MinSpeed - MaxSpeed) + MaxSpeed

Small Grid
- Mass range: 10,000 - 500,000kg
- Speed range: 140 - 200m/s
A small grid with a mass of 10,000kg or less will go 200m/s.
A small grid with a mass of 500,000kg or more will go 140m/s.

Large Grid
- Mass range: 100,000 - 10,000,000kg
- Speed range: 100 - 160m/s
A large grid with a mass of 100,000kg or less will go 160m/s.
A large grid with a mass of 10,000,000kg or more will go 100m/s.
*/