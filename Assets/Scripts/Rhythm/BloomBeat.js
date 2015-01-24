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
	move = Mathf.FloorToInt(beatTime)%2 == 0;
	var time : float = beatTime - Mathf.Floor(beatTime);
	if(move) time = Mathf.Abs(Mathf.Sin(time*Mathf.PI));
	else time = 0.0;
	flares.sepBlurSpread = Mathf.Lerp(0.1, blurSpread,time);

}

function BeatTime(_beatTime : float) {
    beatTime = _beatTime;
}
