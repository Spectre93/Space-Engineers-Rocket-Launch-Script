	// The tag on the ship's piston block (cockpit/remote control), 
	// enclosed in square brackets '[ ]'
	const string _pistonTag = "Super Piston";
	const string _lcdTag	= "Super Piston Cockpit";

	IMyPistonBase piston;
	IMyTextSurface lcd;
	float curr = 0;

	public Program(){
		List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
		GridTerminalSystem.SearchBlocksOfName("[" + _pistonTag + "]", blocks);
		try{
			piston = blocks[0] as IMyPistonBase;
			piston.MaxLimit = 0;
			piston.MinLimit = 0;
			piston.Retract();
		}catch(Exception e){ 
			Echo("No piston marked as '[" + _pistonTag + "]' found.");
		}

		GridTerminalSystem.SearchBlocksOfName("[" + _lcdTag + "]", blocks);
				try{
			var temp = blocks[0] as IMyTextSurfaceProvider;
			lcd = ((IMyTextSurfaceProvider)temp).GetSurface(2);
		}catch(Exception e){ 
			Echo("No lcd marked as '[" + _lcdTag + "]' found.");
		}
	}

	public void Main(string argument){
		if(piston == null) return;
		float arg = Single.Parse(argument);
		if(arg < 0){
			piston.MinLimit= curr + arg;
			curr+= arg;
			if(curr < piston.LowestPosition) curr = piston.LowestPosition;
			piston.Retract();
		}else if(arg >0){
			piston.MaxLimit= curr + arg;
			curr+= arg;
			if(curr > piston.HighestPosition) curr = piston.HighestPosition;
			piston.Extend();
		}
		if(lcd == null) return;
		lcd.WriteText(curr + "m.");
	}