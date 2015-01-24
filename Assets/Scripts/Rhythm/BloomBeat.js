#pragma strict

	private var move : boolean = false;
	private var beatTime : float = 0.0;
	public var myCompassBeat : float = 1f;
	
	private var previousEventTime : float;
	private var nextEventTime : float;
	
	private var flares : BloomAndLensFlares; 
	private var blurSpread : float;


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
		move = !move;
		beatTime += 1.0/myCompassBeat;
		if(!move) return;
		if (flares != null) flares.sepBlurSpread = 0.1;
		
		previousEventTime = data.y;
		nextEventTime = previousEventTime + (60f/(90*myCompassBeat));


	}
}
