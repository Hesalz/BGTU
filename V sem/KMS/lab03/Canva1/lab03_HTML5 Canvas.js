(function (cjs, an) {

var p; // shortcut to reference prototypes
var lib={};var ss={};var img={};
lib.ssMetadata = [
		{name:"lab03_HTML5 Canvas_atlas_1", frames: [[294,0,135,70],[0,132,223,70],[225,132,180,70],[0,0,292,130]]}
];


(lib.AnMovieClip = function(){
	this.actionFrames = [];
	this.ignorePause = false;
	this.currentSoundStreamInMovieclip;
	this.soundStreamDuration = new Map();
	this.streamSoundSymbolsList = [];

	this.gotoAndPlayForStreamSoundSync = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndPlay.call(this,positionOrLabel);
	}
	this.gotoAndPlay = function(positionOrLabel){
		this.clearAllSoundStreams();
		var pos = this.timeline.resolve(positionOrLabel);
		if (pos != null) { this.startStreamSoundsForTargetedFrame(pos); }
		cjs.MovieClip.prototype.gotoAndPlay.call(this,positionOrLabel);
	}
	this.play = function(){
		this.clearAllSoundStreams();
		this.startStreamSoundsForTargetedFrame(this.currentFrame);
		cjs.MovieClip.prototype.play.call(this);
	}
	this.gotoAndStop = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndStop.call(this,positionOrLabel);
		this.clearAllSoundStreams();
	}
	this.stop = function(){
		cjs.MovieClip.prototype.stop.call(this);
		this.clearAllSoundStreams();
	}
	this.startStreamSoundsForTargetedFrame = function(targetFrame){
		for(var index=0; index<this.streamSoundSymbolsList.length; index++){
			if(index <= targetFrame && this.streamSoundSymbolsList[index] != undefined){
				for(var i=0; i<this.streamSoundSymbolsList[index].length; i++){
					var sound = this.streamSoundSymbolsList[index][i];
					if(sound.endFrame > targetFrame){
						var targetPosition = Math.abs((((targetFrame - sound.startFrame)/lib.properties.fps) * 1000));
						var instance = playSound(sound.id);
						var remainingLoop = 0;
						if(sound.offset){
							targetPosition = targetPosition + sound.offset;
						}
						else if(sound.loop > 1){
							var loop = targetPosition /instance.duration;
							remainingLoop = Math.floor(sound.loop - loop);
							if(targetPosition == 0){ remainingLoop -= 1; }
							targetPosition = targetPosition % instance.duration;
						}
						instance.loop = remainingLoop;
						instance.position = Math.round(targetPosition);
						this.InsertIntoSoundStreamData(instance, sound.startFrame, sound.endFrame, sound.loop , sound.offset);
					}
				}
			}
		}
	}
	this.InsertIntoSoundStreamData = function(soundInstance, startIndex, endIndex, loopValue, offsetValue){ 
 		this.soundStreamDuration.set({instance:soundInstance}, {start: startIndex, end:endIndex, loop:loopValue, offset:offsetValue});
	}
	this.clearAllSoundStreams = function(){
		this.soundStreamDuration.forEach(function(value,key){
			key.instance.stop();
		});
 		this.soundStreamDuration.clear();
		this.currentSoundStreamInMovieclip = undefined;
	}
	this.stopSoundStreams = function(currentFrame){
		if(this.soundStreamDuration.size > 0){
			var _this = this;
			this.soundStreamDuration.forEach(function(value,key,arr){
				if((value.end) == currentFrame){
					key.instance.stop();
					if(_this.currentSoundStreamInMovieclip == key) { _this.currentSoundStreamInMovieclip = undefined; }
					arr.delete(key);
				}
			});
		}
	}

	this.computeCurrentSoundStreamInstance = function(currentFrame){
		if(this.currentSoundStreamInMovieclip == undefined){
			var _this = this;
			if(this.soundStreamDuration.size > 0){
				var maxDuration = 0;
				this.soundStreamDuration.forEach(function(value,key){
					if(value.end > maxDuration){
						maxDuration = value.end;
						_this.currentSoundStreamInMovieclip = key;
					}
				});
			}
		}
	}
	this.getDesiredFrame = function(currentFrame, calculatedDesiredFrame){
		for(var frameIndex in this.actionFrames){
			if((frameIndex > currentFrame) && (frameIndex < calculatedDesiredFrame)){
				return frameIndex;
			}
		}
		return calculatedDesiredFrame;
	}

	this.syncStreamSounds = function(){
		this.stopSoundStreams(this.currentFrame);
		this.computeCurrentSoundStreamInstance(this.currentFrame);
		if(this.currentSoundStreamInMovieclip != undefined){
			var soundInstance = this.currentSoundStreamInMovieclip.instance;
			if(soundInstance.position != 0){
				var soundValue = this.soundStreamDuration.get(this.currentSoundStreamInMovieclip);
				var soundPosition = (soundValue.offset?(soundInstance.position - soundValue.offset): soundInstance.position);
				var calculatedDesiredFrame = (soundValue.start)+((soundPosition/1000) * lib.properties.fps);
				if(soundValue.loop > 1){
					calculatedDesiredFrame +=(((((soundValue.loop - soundInstance.loop -1)*soundInstance.duration)) / 1000) * lib.properties.fps);
				}
				calculatedDesiredFrame = Math.floor(calculatedDesiredFrame);
				var deltaFrame = calculatedDesiredFrame - this.currentFrame;
				if((deltaFrame >= 0) && this.ignorePause){
					cjs.MovieClip.prototype.play.call(this);
					this.ignorePause = false;
				}
				else if(deltaFrame >= 2){
					this.gotoAndPlayForStreamSoundSync(this.getDesiredFrame(this.currentFrame,calculatedDesiredFrame));
				}
				else if(deltaFrame <= -2){
					cjs.MovieClip.prototype.stop.call(this);
					this.ignorePause = true;
				}
			}
		}
	}
}).prototype = p = new cjs.MovieClip();
// symbols:



(lib.CachedBmp_6 = function() {
	this.initialize(ss["lab03_HTML5 Canvas_atlas_1"]);
	this.gotoAndStop(0);
}).prototype = p = new cjs.Sprite();



(lib.CachedBmp_4 = function() {
	this.initialize(ss["lab03_HTML5 Canvas_atlas_1"]);
	this.gotoAndStop(1);
}).prototype = p = new cjs.Sprite();



(lib.CachedBmp_2 = function() {
	this.initialize(ss["lab03_HTML5 Canvas_atlas_1"]);
	this.gotoAndStop(2);
}).prototype = p = new cjs.Sprite();



(lib.CachedBmp_5 = function() {
	this.initialize(ss["lab03_HTML5 Canvas_atlas_1"]);
	this.gotoAndStop(3);
}).prototype = p = new cjs.Sprite();



(lib.Ус = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(2,1,1).p("AAFgvQgDABgCAAQgSAAgOgLQgPgKAAgPQAAgPAPgLQAOgKASAAQAUAAAOAKQAOALAAAPQAAAPgOAKQgNAKgQAAIAACm");
	this.shape.setTransform(-28.45,-40.875);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#999999").s().p("AggAaQgOgLgBgPQABgOAOgLQANgKATAAQATAAAOAKQAPALAAAOQAAAPgPALQgMAJgQABIgFAAQgTAAgNgKg");
	this.shape_1.setTransform(-28.45,-49.2);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f().s("#000000").ss(2,1,1).p("AACguQgTgBgNgMQgNgLABgPQABgPAOgJQAPgKASACQAUABANALQALApgPAKQgMAIgRAAQgCAAgCAAgAAGguIgHB7QAFh7gCAAAgEB2IADgp");
	this.shape_2.setTransform(-27.5039,-40.7852);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f("#999999").s().p("AACAjQgTgBgNgLQgNgLABgOQABgPAOgKQAPgJASABQAUABANAMQALAogPAJQgMAJgRAAIgEgBg");
	this.shape_3.setTransform(-27.5039,-49.0352);

	this.shape_4 = new cjs.Shape();
	this.shape_4.graphics.f().s("#000000").ss(2,1,1).p("AAAgtQgTgDgNgMQgMgMACgPQACgPAPgIQAPgJASADQATACANAMQAIAqgPAJQgNAHgRgBQgCAAgBAAgAADgtIgPB6QANh6gBAAAgRB2QADgVACgU");
	this.shape_4.setTransform(-26.1856,-40.7284);

	this.shape_5 = new cjs.Shape();
	this.shape_5.graphics.f("#999999").s().p("AADAkIgDgBQgTgCgNgNQgMgMACgNQACgPAPgJQAPgIASACQATADANAMQAIApgPAIQgMAHgOAAIgEAAg");
	this.shape_5.setTransform(-26.1856,-48.8975);

	this.shape_6 = new cjs.Shape();
	this.shape_6.graphics.f().s("#000000").ss(2,1,1).p("AgCgtQgTgEgMgNQgLgMADgPQACgPAQgIQAQgHARAEQAUAEALANQAGAqgQAHQgNAHgRgCQgBAAgCgBgAABgsIgYB4QAXh4gCgBAgeB1QAEgVADgU");
	this.shape_6.setTransform(-24.8742,-40.6727);

	this.shape_7 = new cjs.Shape();
	this.shape_7.graphics.f("#999999").s().p("AABAkIgDgBQgTgEgMgNQgLgMADgOQACgPAQgIQAQgHARAEQAUAEALANQAGApgQAHQgKAFgLAAIgJAAg");
	this.shape_7.setTransform(-24.8742,-48.7648);

	this.shape_8 = new cjs.Shape();
	this.shape_8.graphics.f().s("#000000").ss(2,1,1).p("AgDgrQgCgBgDAAQgSgFgLgOQgLgOAEgOQAEgPAQgHQAQgGASAFQATAFALAOQAKAOgDAOQgEAPgRAGQgOAGgPgDIgrCg");
	this.shape_8.setTransform(-23.2562,-40.6386);

	this.shape_9 = new cjs.Shape();
	this.shape_9.graphics.f("#999999").s().p("AgEAkIgFgBQgSgFgLgOQgLgOAEgNQAEgPAQgHQAQgGASAFQATAFALAOQAKAOgDANQgEAPgRAGQgIAEgKAAIgLgBg");
	this.shape_9.setTransform(-23.1864,-48.6242);

	this.shape_10 = new cjs.Shape();
	this.shape_10.graphics.f().s("#000000").ss(2,1,1).p("AgBgtQgTgDgMgMQgLgMACgPQACgOAQgJQAPgHARADQATADAMAMQAHApgPAIQgNAHgRgBQgCgBgBAAgAACgsIgSB4QAQh5gBAAAgWB0QADgVADgT");
	this.shape_10.setTransform(-25.6634,-40.5429);

	this.shape_11 = new cjs.Shape();
	this.shape_11.graphics.f("#999999").s().p("AACAjIgDAAQgTgDgMgNQgLgMACgNQACgPAQgIQAPgIARADQATADAMAMQAHApgPAIQgLAGgNAAIgGgBg");
	this.shape_11.setTransform(-25.6634,-48.5796);

	this.shape_12 = new cjs.Shape();
	this.shape_12.graphics.f().s("#000000").ss(2,1,1).p("AABgtQgRgBgNgLQgNgKABgPQAAgOAOgKQAOgJASABQATABANAKQALAogOAKQgMAIgQAAQgCAAgDAAgAAGgtIgGB4QAEh4gDAAAgCBzQABgVABgT");
	this.shape_12.setTransform(-27.7443,-40.48);

	this.shape_13 = new cjs.Shape();
	this.shape_13.graphics.f("#999999").s().p("AABAjQgRgBgNgLQgNgLABgNQAAgPAOgKQAOgJASABQATABANALQALAngOAJQgMAJgQAAIgFAAg");
	this.shape_13.setTransform(-27.7443,-48.53);

	this.shape_14 = new cjs.Shape();
	this.shape_14.graphics.f().s("#000000").ss(2,1,1).p("AAFgtQgSABgOgJQgOgJgBgPQAAgOANgLQANgLARgBQATgBAOAJQAQAngNALQgMAJgQACQgCAAgCAAgAAJgtIAGB4QgIh4gCAAAASBzQgBgVgCgT");
	this.shape_14.setTransform(-29.8294,-40.4798);

	this.shape_15 = new cjs.Shape();
	this.shape_15.graphics.f("#999999").s().p("AgbAbQgOgKgBgOQAAgOANgLQANgLARgBQATgBAOAKQAQAmgNAKQgMAKgQACIgEAAIgEAAQgPAAgNgIg");
	this.shape_15.setTransform(-29.8294,-48.5248);

	this.shape_16 = new cjs.Shape();
	this.shape_16.graphics.f().s("#000000").ss(2,1,1).p("AAIgsQgRADgPgIQgQgIgCgOQgCgPALgMQAMgNATgDQASgDAPAIQAUAmgMAMQgLALgPADQgCABgDAAgAANgtIATB4QgWh3gCAAAAmBzQgDgUgDgU");
	this.shape_16.setTransform(-31.8799,-40.4909);

	this.shape_17 = new cjs.Shape();
	this.shape_17.graphics.f("#999999").s().p("AgYAeQgQgIgCgOQgCgOALgMQAMgNATgDQASgDAPAIQAUAlgMAMQgLALgPADIgFABIgJABQgMAAgLgGg");
	this.shape_17.setTransform(-31.8799,-48.4731);

	this.shape_18 = new cjs.Shape();
	this.shape_18.graphics.f().s("#000000").ss(2,1,1).p("AAJgsQgCAAgDABQgRAFgQgGQgRgHgEgOQgDgPAKgOQALgOATgFQASgFAQAHQAQAHAEAOQAEAPgLANQgKAMgPAGIArCg");
	this.shape_18.setTransform(-33.2187,-40.5337);

	this.shape_19 = new cjs.Shape();
	this.shape_19.graphics.f("#999999").s().p("AgYAiQgRgHgEgOQgDgOAKgOQALgOATgFQASgFAQAHQAQAHAEAOQAEAOgLANQgKAMgPAGIgFABQgIACgHAAQgKAAgIgDg");
	this.shape_19.setTransform(-33.7136,-48.4201);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_1},{t:this.shape}]}).to({state:[{t:this.shape_3},{t:this.shape_2}]},1).to({state:[{t:this.shape_5},{t:this.shape_4}]},1).to({state:[{t:this.shape_7},{t:this.shape_6}]},1).to({state:[{t:this.shape_9},{t:this.shape_8}]},1).to({state:[{t:this.shape_11},{t:this.shape_10}]},1).to({state:[{t:this.shape_13},{t:this.shape_12}]},1).to({state:[{t:this.shape_15},{t:this.shape_14}]},1).to({state:[{t:this.shape_17},{t:this.shape_16}]},1).to({state:[{t:this.shape_19},{t:this.shape_18}]},1).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-39.4,-53.8,21.9,25.9);


(lib.Тело = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(2,1,1).p("ALSBpQg3A5h1AvQjbBWk0AAQk0AAjahWQjahXAAh6QAAh5DahXQAWgIAXgIQBjgiB2gSQB7gSCNAAQE0AADbBWQBeAmA2As");
	this.shape.setTransform(72.2,29.5);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#999999").s().p("An3DQQjahWAAh6QAAh5DahXIAtgQQBkgiB1gRQB7gTCNAAQE1AADaBWQBeAmA2AsQgQAqgBAyQABAyARAtQAJAXAOAVQg2A5h2AuQjaBXk1AAQk0AAjahXg");
	this.shape_1.setTransform(72.2,29.5);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_1},{t:this.shape}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-1,-1,146.4,61);


(lib.Пауза = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.instance = new lib.CachedBmp_6();
	this.instance.setTransform(-37.95,-21.5,0.5,0.5);

	this.instance_1 = new lib.CachedBmp_5();
	this.instance_1.setTransform(-72.5,-32.5,0.5,0.5);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_1},{t:this.instance}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-72.5,-32.5,146,65);


(lib.Лапа = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(3,1,1).p("Ah3kmIDvEOIAAE/");
	this.shape.setTransform(27,43.5);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f().s("#000000").ss(3,1,1).p("AhtkrIDbEbIgVE8");
	this.shape_1.setTransform(30.075,42.975);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f().s("#000000").ss(3,1,1).p("AhjkxIDHEpIgpE6");
	this.shape_2.setTransform(33.125,42.45);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f().s("#000000").ss(3,1,1).p("Ahak2IC1E2Ig/E3");
	this.shape_3.setTransform(36.175,41.925);

	this.shape_4 = new cjs.Shape();
	this.shape_4.graphics.f().s("#000000").ss(3,1,1).p("AhQk7IChFCIhSE1");
	this.shape_4.setTransform(39.225,41.4);

	this.shape_5 = new cjs.Shape();
	this.shape_5.graphics.f().s("#000000").ss(3,1,1).p("AhekvIC9ErIgyE0");
	this.shape_5.setTransform(34.525,42.65);

	this.shape_6 = new cjs.Shape();
	this.shape_6.graphics.f().s("#000000").ss(3,1,1).p("AhskiIDZERIgRE0");
	this.shape_6.setTransform(29.775,43.875);

	this.shape_7 = new cjs.Shape();
	this.shape_7.graphics.f().s("#000000").ss(3,1,1).p("AiCkWID1D5IAQE0");
	this.shape_7.setTransform(25.9,45.125);

	this.shape_8 = new cjs.Shape();
	this.shape_8.graphics.f().s("#000000").ss(3,1,1).p("AihkKIERDgIAyE1");
	this.shape_8.setTransform(22.825,46.35);

	this.shape_9 = new cjs.Shape();
	this.shape_9.graphics.f().s("#000000").ss(3,1,1).p("Ai/j9IEsDHIBTE0");
	this.shape_9.setTransform(19.775,47.6);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape}]}).to({state:[{t:this.shape_1}]},1).to({state:[{t:this.shape_2}]},1).to({state:[{t:this.shape_3}]},1).to({state:[{t:this.shape_4}]},1).to({state:[{t:this.shape_5}]},1).to({state:[{t:this.shape_6}]},1).to({state:[{t:this.shape_7}]},1).to({state:[{t:this.shape_8}]},1).to({state:[{t:this.shape_9}]},1).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-0.9,8.3,49.699999999999996,66.2);


(lib.Запуск = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.instance = new lib.CachedBmp_4();
	this.instance.setTransform(-59.95,-21.45,0.5,0.5);

	this.instance_1 = new lib.CachedBmp_5();
	this.instance_1.setTransform(-73.5,-32.45,0.5,0.5);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_1},{t:this.instance}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-73.5,-32.4,146,65);


(lib.Возврат = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.instance = new lib.CachedBmp_2();
	this.instance.setTransform(-45.95,-21.5,0.5,0.5);

	this.instance_1 = new lib.CachedBmp_5();
	this.instance_1.setTransform(-72.5,-32.5,0.5,0.5);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_1},{t:this.instance}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-72.5,-32.5,146,65);


(lib.Жук = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// Слой_1
	this.instance = new lib.Ус("synched",7);
	this.instance.setTransform(90.7,2.55,1,1,0,116.0466,-63.9534,-27,-30.2);

	this.instance_1 = new lib.Ус("synched",0);
	this.instance_1.setTransform(86.45,-16.85,1,1,45,0,0,-26.9,-30.1);

	this.instance_2 = new lib.Лапа("synched",9);
	this.instance_2.setTransform(-91.9,54.05,1,1,0,-165.0017,14.9983,16.9,34.8);

	this.instance_3 = new lib.Лапа("synched",0);
	this.instance_3.setTransform(-56.2,64.05,1,1,0,180,0,16.9,34.8);

	this.instance_4 = new lib.Лапа("synched",6);
	this.instance_4.setTransform(-15.7,64.05,1,1,0,180,0,16.9,34.8);

	this.instance_5 = new lib.Лапа("synched",0);
	this.instance_5.setTransform(-96.15,-50.85,1,1,-14.9983,0,0,16.9,34.8);

	this.instance_6 = new lib.Лапа("synched",3);
	this.instance_6.setTransform(-60.45,-60.85,1,1,0,0,0,16.9,34.8);

	this.instance_7 = new lib.Лапа("synched",0);
	this.instance_7.setTransform(-19.95,-60.85,1,1,0,0,0,16.9,34.8);

	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(2,1,1).p("AHiBpQgOgVgJgXQgRgtAAgyQAAgyAQgqAHiBpQg2A5h2AuQjaBXk0AAQk0AAjbhXQjahWAAh6QAAh5DahXQAWgIAXgIQBkgiB1gSQB7gSCOAAQE0AADaBWQBeAmA2AsQACgGACgFQATgsAlglQBMhMBsAAQBrAABMBMQBNBMAABsQAABqhNBMQhMBNhrAAQhsAAhMhNQgVgUgPgXgAMEhjQAAAOgLAKQgKALgOAAQgPAAgKgLQgKgKAAgOQAAgPAKgKQAKgKAPAAQAOAAAKAKQALAKAAAPgAMlCDQAAAPgLALQgLALgPAAQgPAAgLgLQgKgLAAgPQAAgPAKgLQALgKAPAAQAPAAALAKQALALAAAPg");
	this.shape.setTransform(-7.175,1.35);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#999999").s().p("ArnDRQjahXAAh6QAAh5DahXIAtgQQBkgiB1gRQB7gTCOAAQE0AADaBWQBeAmA2AsIAEgLQATgsAlglQBMhMBsAAQBrAABMBMQBNBMAABsQAABqhNBMQhMBNhrAAQhsAAhMhNQgVgUgPgXQgOgVgJgXQgRgtAAgyQAAgyAQgqQgQAqAAAyQAAAyARAtQAJAXAOAVQg2A5h2AvQjaBWk0AAQk0AAjbhWgALcCDQAAAPAKALQALALAPgBQAPABALgLQALgLAAgPQAAgPgLgLQgLgLgPABQgPgBgLALQgKALAAAPIAAAAgAK+hjQAAAOAKAKQAKALAPAAQAOAAAKgLQALgKAAgOQAAgOgLgLQgKgKgOAAQgPAAgKAKQgKALAAAOIAAAAgALmCdQgKgLAAgPQAAgPAKgLQALgLAPABQAPgBALALQALALAAAPQAAAPgLALQgLALgPgBQgPABgLgLgAMlCDIAAAAgALIhLQgKgKAAgOQAAgOAKgLQAKgKAPAAQAOAAAKAKQALALAAAOQAAAOgLAKQgKALgOAAQgPAAgKgLgAMEhjIAAAAg");
	this.shape_1.setTransform(-7.175,1.35);

	this.instance_8 = new lib.Тело("synched",0);
	this.instance_8.setTransform(-31.15,1.35,1,1,0,0,0,72.2,29.5);

	this.shape_2 = new cjs.Shape();
	this.shape_2.graphics.f().s("#000000").ss(2,1,1).p("AjzhbQACgGACgFQATgsAlglQBMhMBrAAQBsAABLBMQBNBMAABrQAABrhNBMQhLBNhsAAQhrAAhMhNQgVgUgPgXQgOgVgJgXQgRgtAAgzQAAgxAQgqgABGhAQAAAOgKAKQgKALgPAAQgPAAgKgLQgKgKAAgOQAAgPAKgKQAKgKAPAAQAPAAAKAKQAKAKAAAPgABnCmQAAAPgLALQgKALgPAAQgPAAgMgLQgKgLAAgPQAAgPAKgLQAMgKAPAAQAPAAAKAKQALALAAAPg");
	this.shape_2.setTransform(63,-2.15);

	this.shape_3 = new cjs.Shape();
	this.shape_3.graphics.f("#999999").s().p("Ai3C3QgVgUgPgXQgOgVgIgXQgSgtAAgzQAAgxAQgqIAFgLQASgsAlglQBMhMBrAAQBrAABNBMQBMBMAABrQAABrhMBMQhNBNhrAAQhrAAhMhNgAAeCmQAAAPAKALQAMALAPgBQAOABALgLQALgLAAgPQAAgPgLgLQgLgLgOABQgPgBgMALQgKALAAAPIAAAAgAAAhAQAAAOAKAKQALALAOAAQAOAAAKgLQALgKAAgOQAAgOgLgLQgKgKgOAAQgOAAgLAKQgKALAAAOIAAAAgAAoDAQgKgLAAgPQAAgPAKgLQAMgLAPABQAOgBALALQALALAAAPQAAAPgLALQgLALgOgBQgPABgMgLgABnCmIAAAAgAAKgoQgKgKAAgOQAAgOAKgLQALgKAOAAQAOAAAKAKQALALAAAOQAAAOgLAKQgKALgOAAQgOAAgLgLgABGhAIAAAAg");
	this.shape_3.setTransform(63,-2.15);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.shape_1},{t:this.shape},{t:this.instance_7,p:{startPosition:0}},{t:this.instance_6,p:{startPosition:3}},{t:this.instance_5,p:{startPosition:0}},{t:this.instance_4,p:{startPosition:6}},{t:this.instance_3,p:{startPosition:0}},{t:this.instance_2,p:{startPosition:9}},{t:this.instance_1,p:{startPosition:0}},{t:this.instance,p:{startPosition:7}}]}).to({state:[{t:this.shape_3},{t:this.shape_2},{t:this.instance_7,p:{startPosition:9}},{t:this.instance_6,p:{startPosition:2}},{t:this.instance_5,p:{startPosition:9}},{t:this.instance_4,p:{startPosition:5}},{t:this.instance_3,p:{startPosition:9}},{t:this.instance_2,p:{startPosition:8}},{t:this.instance_1,p:{startPosition:9}},{t:this.instance,p:{startPosition:6}},{t:this.instance_8}]},9).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-116.6,-87.3,230.2,177.89999999999998);


// stage content:
(lib.lab03_HTML5Canvas = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	this.actionFrames = [0,39];
	this.streamSoundSymbolsList[0] = [{id:"zhykLETIT",startFrame:0,endFrame:80,loop:1,offset:0}];
	this.streamSoundSymbolsList[39] = [{id:"DTP",startFrame:39,endFrame:80,loop:1,offset:0}];
	// timeline functions:
	this.frame_0 = function() {
		this.clearAllSoundStreams();
		 
		var soundInstance = playSound("zhykLETIT",0);
		this.InsertIntoSoundStreamData(soundInstance,0,80,1);
		this.stop(); // Останавливает анимацию на первом кадре
		
		// Обработчики кнопок — не забудьте дать кнопкам имена экземпляров на сцене
		this.buttonStop.addEventListener("click", () => {
		    this.stop();
		});
		
		this.buttonPlay.addEventListener("click", () => {
		    this.play();
		});
		
		this.buttonReplay.addEventListener("click", () => {
		    this.gotoAndStop(0); // возрат к первому кадру (нумерация с 0)
		});
		/* import flash.events.MouseEvent;
		
		stop(); // Остановить анимацию на первом кадре
		
		buttonStop.addEventListener(MouseEvent.CLICK, onStopClick);
		buttonPlay.addEventListener(MouseEvent.CLICK, onPlayClick);
		buttonReplay.addEventListener(MouseEvent.CLICK, onRewindClick);
		
		function onStopClick(e:MouseEvent):void {
		    stop();
		}
		
		function onPlayClick(e:MouseEvent):void {
		    play();
		}
		
		function onRewindClick(e:MouseEvent):void {
		    gotoAndStop(1);
		}*/
	}
	this.frame_39 = function() {
		var soundInstance = playSound("DTP",0);
		this.InsertIntoSoundStreamData(soundInstance,39,80,1);
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).call(this.frame_0).wait(39).call(this.frame_39).wait(41));

	// Слой_7
	this.buttonStop = new lib.Пауза();
	this.buttonStop.name = "buttonStop";
	this.buttonStop.setTransform(1005.25,42.2);
	new cjs.ButtonHelper(this.buttonStop, 0, 1, 1);

	this.buttonPlay = new lib.Запуск();
	this.buttonPlay.name = "buttonPlay";
	this.buttonPlay.setTransform(838.15,42.15);
	new cjs.ButtonHelper(this.buttonPlay, 0, 1, 1);

	this.buttonReplay = new lib.Возврат();
	this.buttonReplay.name = "buttonReplay";
	this.buttonReplay.setTransform(1175.85,41.45);
	new cjs.ButtonHelper(this.buttonReplay, 0, 1, 1);

	this.instance = new lib.Пауза();
	this.instance.setTransform(1005.25,42.2);
	new cjs.ButtonHelper(this.instance, 0, 1, 1);

	this.instance_1 = new lib.Запуск();
	this.instance_1.setTransform(838.15,42.15);
	new cjs.ButtonHelper(this.instance_1, 0, 1, 1);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.buttonReplay},{t:this.buttonPlay},{t:this.buttonStop}]}).to({state:[{t:this.buttonReplay},{t:this.instance_1},{t:this.instance}]},79).wait(1));

	// жуки
	this.instance_2 = new lib.Жук();
	this.instance_2.setTransform(188.7,767.95,0.7152,0.7152,-44.9983,0,0,1.5,1.8);

	this.instance_3 = new lib.Жук();
	this.instance_3.setTransform(1768,500.45,1.5719,1.5719,90,0,0,1.5,1.6);

	this.instance_4 = new lib.Жук();
	this.instance_4.setTransform(781.8,651.65,1,1,74.9998,0,0,1.5,1.7);

	this.instance_5 = new lib.Жук();
	this.instance_5.setTransform(315.7,255.55,0.5514,0.6448,120.0015,0,0,1.5,1.4);

	this.instance_6 = new lib.Жук();
	this.instance_6.setTransform(1079.9,175.75,0.4303,0.4303,-44.9986,0,0,1.4,1.8);

	this.instance_7 = new lib.Жук();
	this.instance_7.setTransform(129.45,356.15,1,1,0,0,0,1.4,1.6);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance_7},{t:this.instance_6},{t:this.instance_5},{t:this.instance_4},{t:this.instance_3},{t:this.instance_2}]}).to({state:[{t:this.instance_7},{t:this.instance_6},{t:this.instance_5},{t:this.instance_4},{t:this.instance_3},{t:this.instance_2}]},79).wait(1));

	// Слой_1
	this.instance_8 = new lib.Жук();
	this.instance_8.setTransform(1944.85,0.25,0.9038,0.9038,135,0,0,1.4,1.6);

	this.timeline.addTween(cjs.Tween.get(this.instance_8).to({rotation:105.3793,guide:{path:[1944.8,0.3,1944.8,0.3,1944.9,0.3,1923.4,55.4,1863.2,92,1764.7,151.9,1653.1,169.7,1610.1,176.5,1568.6,190.1,1546.3,197.4,1526.7,210.3,1500.5,236.2,1465.3,257.4,1442.9,270.9,1418.9,280.7,1417.2,284.8,1413.7,287.3,1403.3,294.4,1393,301.8,1390.5,303.8,1387.8,305.4,1386.5,306.2,1385.1,307,1379.5,310.2,1373.4,311.6,1348.2,333.8,1317.5,351.3,1266.9,380.2,1213.6,403.8,1261.8,504.9,1300.3,610.4]}},39).to({regX:-34.4,regY:-5.5,scaleY:1.1012,rotation:0,skewX:-78.4167,skewY:101.5827,guide:{path:[1300.3,610.4,1298.6,605.6,1296.8,600.8]}},5).to({regX:1.5,regY:1.6,scaleX:0.9037,scaleY:0.9037,rotation:97.7864,skewX:0,skewY:0,guide:{path:[1296.8,600.8,1301.2,612.8,1305.5,624.8]}},5).to({scaleX:0.9038,scaleY:0.9038,rotation:75.0023,guide:{path:[1305.6,624.8,1308.5,633,1311.4,641.2,1323.7,676.5,1318.2,714,1300.6,834.7,1269.6,953,1247.2,1038.9,1163.8,1059.2,1104.6,1073.4,1049.7,1045.3,938.3,988.1,819.2,948.7,809.8,948.4,800.4,948.1,769.7,972.1,746.1,1002.5,741.5,1008.5,736.9,1014.6,734.3,1040.4,731.8,1066.1,737.7,1086.6,752,1101.1,758.8,1107.9,757.1,1116.9,757,1117.4,756.8,1118,756.3,1120.1,754.2,1121.4,753.8,1121.5,753.5,1121.6,752.7,1121.9,751.9,1122.1,751.9,1122.1,751.9,1122.1]}},30).wait(1));

	// Слой_3
	this.instance_9 = new lib.Жук();
	this.instance_9.setTransform(-135.95,1022.65,1.4277,1.4277,0,0,0,1.4,1.7);

	this.timeline.addTween(cjs.Tween.get(this.instance_9).to({regY:1.6,scaleX:1.4276,scaleY:1.4276,rotation:29.9992,guide:{path:[-135.8,1022.5,-136.9,1016,-128.9,1014.7,-120.4,1019.2,-111.8,1023.7,-18.3,1030.6,74.7,1019.9,87.3,1018.4,99.9,1015.8,120.1,1011.4,140.4,1008,152.1,1005.8,163.1,1002.1,173.5,998.5,184.5,997.9,194,997,202.6,992.6,207.7,989.9,213.5,989,223.7,987.7,233.8,984.6,269,973.8,302.8,959.5,309.1,955.2,315.4,950.8,327.1,939.3,338.9,927.7,363.6,880,388.3,832.2,389.6,818.8,398.1,807.6,401.2,803.5,402.3,798.3,404,790.6,407.8,783.3,413.2,773.2,415,761.5,415.9,755.5,418.2,749.6,425.6,731,427,710.7,427.2,707.8,428.4,705,437.5,685.1,440.9,662.8,441.9,656.6,445.4,651.5,449.6,645.3,452.1,638.1,453.9,632.9,459.2,631.3,465.7,617.8,473.8,606.1,484.6,590.5,491.4,573.1,495.8,561.7,499.2,550.4,509.1,517.4,516.6,484,519.9,469.3,521.2,454.2,524.9,409.8,525.3,364.5,525.3,362.8,525.6,361,527.9,347.3,535.6,336.2,536.7,319.9,547.1,306.3,548.8,304.1,549.7,301.3,562.6,283.4,579.7,269.7,587.3,263.5,596.9,262.4,608.3,250.6,624.7,246.5,629.6,245.1,634.3,243,645.6,237.5,658.3,237.8,707.8,217.5,758.7,235.3,766.7,238,775.7,239.5,780,240.3,783.8,242.5,791,246.6,799.1,247.8,810.2,249.7,820.5,254.6,827.4,258.1,834.3,261.5,837.2,262.9,840.4,263.6,843.2,267.3,846.1,271,858.1,282,870.2,292.9,888.6,308.4,907,323.9,915.9,330.1,924.8,336.2,946.7,355.1,965.6,376.1,968.8,379.6,969.1,384.6,986.3,401.3,993.2,423.4,993.9,425.6,994,428.1,1003.1,456.9,1012.2,485.7,1015.2,492.2,1018.3,498.7,1024.2,509.2,1030.2,519.7,1036.3,528,1042.5,536.2,1059.4,554.3,1076.3,572.3,1077.9,576.8,1079.6,581.3,1085.6,588.1,1091.7,594.8,1105.6,611.5,1119.5,628.2,1124.2,635.5,1128.9,642.8,1148,666.8,1167.1,690.7,1171.9,694.4,1176.8,698.1,1201.5,700.2,1223.4,689.2,1229.3,684.7,1235.2,680.2,1236.9,677.3,1238.7,674.4,1243.4,671.6,1248.1,668.7,1260,656.2,1271.9,643.6,1287.9,625.3,1303.9,607,1314.3,593.1,1324.7,579.2,1335.7,568.6,1346.8,558,1352.4,551.7,1358,545.4,1365,539.2,1372,532.9,1389.3,518.4,1410.2,509.4,1412.7,508.3,1415.5,508,1441.9,503.5,1458.8,523.3,1467.6,533.7,1474.1,545.3,1484.6,554.7,1493.1,566.5,1498.6,574.3,1503.3,582.7,1512.6,599.2,1511.7,619.5,1511.2,631.9,1515.7,643.2,1518.5,650.1,1518.6,657.9,1519.5,718.9,1515,778.9,1512.5,812.1,1522,842.3,1534.1,859.4,1546.3,876.5,1552.4,882.4,1558.5,888.2,1567.9,894.6,1577.3,901,1600.4,911.8,1623.4,923.2,1650.1,936.6,1678.4,945,1726.3,959.3,1774.6,973.7,1784.9,976.9,1795.3,980.1,1799,981.2,1801.8,983.8,1834.7,990.7,1866.8,1001,1894.7,1009.9,1922.6,1017.2,1949,1024.1,1973.6,1036,1978.8,1037.2,1984.1,1038.3,1993.6,1049,2008.3,1051.9,2010.4,1052.5,2012.6,1053.7,2024.9,1060.8,2037.4,1066.7,2060.4,1077.6,2084.7,1088,2107.9,1098,2125.8,1115.7,2125.8,1115.7,2125.8,1115.7]}},59).to({_off:true},1).wait(20));

	this._renderFirstFrame();

}).prototype = p = new lib.AnMovieClip();
p.nominalBounds = new cjs.Rectangle(661.1,428,1617.3000000000002,797.4000000000001);
// library properties:
lib.properties = {
	id: '105C6BB68D423246BA66718698249226',
	width: 1920,
	height: 1080,
	fps: 30,
	color: "#FFFFFF",
	opacity: 1.00,
	manifest: [
		{src:"images/lab03_HTML5 Canvas_atlas_1.png?1758691085112", id:"lab03_HTML5 Canvas_atlas_1"},
		{src:"sounds/DTP.mp3?1758691085135", id:"DTP"},
		{src:"sounds/zhykLETIT.mp3?1758691085135", id:"zhykLETIT"}
	],
	preloads: []
};



// bootstrap callback support:

(lib.Stage = function(canvas) {
	createjs.Stage.call(this, canvas);
}).prototype = p = new createjs.Stage();

p.setAutoPlay = function(autoPlay) {
	this.tickEnabled = autoPlay;
}
p.play = function() { this.tickEnabled = true; this.getChildAt(0).gotoAndPlay(this.getTimelinePosition()) }
p.stop = function(ms) { if(ms) this.seek(ms); this.tickEnabled = false; }
p.seek = function(ms) { this.tickEnabled = true; this.getChildAt(0).gotoAndStop(lib.properties.fps * ms / 1000); }
p.getDuration = function() { return this.getChildAt(0).totalFrames / lib.properties.fps * 1000; }

p.getTimelinePosition = function() { return this.getChildAt(0).currentFrame / lib.properties.fps * 1000; }

an.bootcompsLoaded = an.bootcompsLoaded || [];
if(!an.bootstrapListeners) {
	an.bootstrapListeners=[];
}

an.bootstrapCallback=function(fnCallback) {
	an.bootstrapListeners.push(fnCallback);
	if(an.bootcompsLoaded.length > 0) {
		for(var i=0; i<an.bootcompsLoaded.length; ++i) {
			fnCallback(an.bootcompsLoaded[i]);
		}
	}
};

an.compositions = an.compositions || {};
an.compositions['105C6BB68D423246BA66718698249226'] = {
	getStage: function() { return exportRoot.stage; },
	getLibrary: function() { return lib; },
	getSpriteSheet: function() { return ss; },
	getImages: function() { return img; }
};

an.compositionLoaded = function(id) {
	an.bootcompsLoaded.push(id);
	for(var j=0; j<an.bootstrapListeners.length; j++) {
		an.bootstrapListeners[j](id);
	}
}

an.getComposition = function(id) {
	return an.compositions[id];
}


an.makeResponsive = function(isResp, respDim, isScale, scaleType, domContainers) {		
	var lastW, lastH, lastS=1;		
	window.addEventListener('resize', resizeCanvas);		
	resizeCanvas();		
	function resizeCanvas() {			
		var w = lib.properties.width, h = lib.properties.height;			
		var iw = window.innerWidth, ih=window.innerHeight;			
		var pRatio = window.devicePixelRatio || 1, xRatio=iw/w, yRatio=ih/h, sRatio=1;			
		if(isResp) {                
			if((respDim=='width'&&lastW==iw) || (respDim=='height'&&lastH==ih)) {                    
				sRatio = lastS;                
			}				
			else if(!isScale) {					
				if(iw<w || ih<h)						
					sRatio = Math.min(xRatio, yRatio);				
			}				
			else if(scaleType==1) {					
				sRatio = Math.min(xRatio, yRatio);				
			}				
			else if(scaleType==2) {					
				sRatio = Math.max(xRatio, yRatio);				
			}			
		}
		domContainers[0].width = w * pRatio * sRatio;			
		domContainers[0].height = h * pRatio * sRatio;
		domContainers.forEach(function(container) {				
			container.style.width = w * sRatio + 'px';				
			container.style.height = h * sRatio + 'px';			
		});
		stage.scaleX = pRatio*sRatio;			
		stage.scaleY = pRatio*sRatio;
		lastW = iw; lastH = ih; lastS = sRatio;            
		stage.tickOnUpdate = false;            
		stage.update();            
		stage.tickOnUpdate = true;		
	}
}
an.handleSoundStreamOnTick = function(event) {
	if(!event.paused){
		var stageChild = stage.getChildAt(0);
		if(!stageChild.paused || stageChild.ignorePause){
			stageChild.syncStreamSounds();
		}
	}
}
an.handleFilterCache = function(event) {
	if(!event.paused){
		var target = event.target;
		if(target){
			if(target.filterCacheList){
				for(var index = 0; index < target.filterCacheList.length ; index++){
					var cacheInst = target.filterCacheList[index];
					if((cacheInst.startFrame <= target.currentFrame) && (target.currentFrame <= cacheInst.endFrame)){
						cacheInst.instance.cache(cacheInst.x, cacheInst.y, cacheInst.w, cacheInst.h);
					}
				}
			}
		}
	}
}


})(createjs = createjs||{}, AdobeAn = AdobeAn||{});
var createjs, AdobeAn;