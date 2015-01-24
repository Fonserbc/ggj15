#pragma strict

	var move : boolean = false;
	var beatTime : float = 0.0;
	public var myCompassBeat : float = 1f;
	
	var previousEventTime : float;
	var nextEventTime : float;
	
	var flares : BloomAndLensFlares; 
	var blurSpread : float;


function Start () {
	flares = gameObject.GetComponent("BloomAndLensFlares")  as BloomAndLensFlares; 
	blurSpread = flares.sepBlurSpread;
	
	previousEventTime = AudioSettings.dspTime;
	nextEventTime = AudioSettings.dspTime;
}

function Update () {
	var time : float = Mathf.Min (1.0,((AudioSettings.dspTime) - previousEventTime) / (nextEventTime - previousEventTime));
	time = Mathf.Abs(Mathf.Sin(time*Mathf.PI));
	flares.sepBlurSpread = Mathf.Lerp(0.1, blurSpread,time);
}

function Beat(data : Vector2) {
	if(data.x == beatTime) {
		move = true;
		beatTime += 1.0/myCompassBeat;
		flares.sepBlurSpread = 0.1;
		
		previousEventTime = data.y;
		nextEventTime = previousEventTime + (60f/(90*myCompassBeat));


	}
}
