/*
	This script is a simple auto leveling script based on Archon's Autolevel script.
	The mathematics in the main function are not of my creation and the credit of 
	which goes solely to Archon. You can find their Autolevel mod, 
	which many more features than this derivative, here:
	https://steamcommunity.com/sharedfiles/filedetails/?id=1366126191
*/

// The tag on the ship's controller block (cockpit/remote control), 
// enclosed in square brackets '[ ]'
string _controller = "Controller";

IMyShipController controller;
List<IMyGyro> gyros;

public Program(){
	gyros = new List<IMyGyro>();

	Runtime.UpdateFrequency = UpdateFrequency.Update10;

	List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("[" + _controller + "]", blocks);
    try{
        controller = blocks[0] as IMyShipController;
    }catch(Exception e){ 
		Echo("No ship controller marked as '[" + _controller + "]' found.");
		Runtime.UpdateFrequency = UpdateFrequency.None;
	}

	GridTerminalSystem.GetBlocksOfType<IMyGyro>(gyros);
	if (gyros.Count == 0) {
		Echo("No gyros found.");
		Runtime.UpdateFrequency = UpdateFrequency.None;
	}
}

public void Main(string argument){
	//Get orientation from ship's controller block
	Matrix orientation;
	controller.Orientation.GetMatrix(out orientation);

	Vector3D down = orientation.Down;
	Vector3D grav = controller.GetNaturalGravity();

	grav.Normalize();
	
	foreach (IMyGyro gyro in gyros) {
		gyro.Orientation.GetMatrix(out orientation);
		Vector3D localDown = Vector3D.Transform(down, MatrixD.Transpose(orientation));
		Vector3D localGrav = Vector3D.Transform(grav, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));
		Vector3D rotation = Vector3D.Cross(localDown, localGrav);

		double angle = rotation.Length();
		angle = Math.Atan2(angle, Math.Sqrt(Math.Max(0.0, 1.0 - angle * angle)));

		double controlCoeff = gyro.GetMaximum<float>("Yaw") * (angle / Math.PI);
		controlCoeff = Math.Min(gyro.GetMaximum<float>("Yaw"), controlCoeff);

		rotation.Normalize();
		rotation *= controlCoeff;

		gyro.SetValueFloat("Pitch", (float) rotation.GetDim(0));
		gyro.SetValueFloat("Yaw", -(float) rotation.GetDim(1));
		gyro.SetValueFloat("Roll", -(float) rotation.GetDim(2));
	}
}