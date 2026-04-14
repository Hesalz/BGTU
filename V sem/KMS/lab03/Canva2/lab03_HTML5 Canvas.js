(function (cjs, an) {

var p; // shortcut to reference prototypes
var lib={};var ss={};var img={};
lib.ssMetadata = [
		{name:"lab03_HTML5 Canvas_atlas_1", frames: [[0,0,769,575]]}
];


(lib.AnMovieClip = function(){
	this.actionFrames = [];
	this.ignorePause = false;
	this.gotoAndPlay = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndPlay.call(this,positionOrLabel);
	}
	this.play = function(){
		cjs.MovieClip.prototype.play.call(this);
	}
	this.gotoAndStop = function(positionOrLabel){
		cjs.MovieClip.prototype.gotoAndStop.call(this,positionOrLabel);
	}
	this.stop = function(){
		cjs.MovieClip.prototype.stop.call(this);
	}
}).prototype = p = new cjs.MovieClip();
// symbols:



(lib.Растровоеизображение3 = function() {
	this.initialize(ss["lab03_HTML5 Canvas_atlas_1"]);
	this.gotoAndStop(0);
}).prototype = p = new cjs.Sprite();



(lib.VokzalKnopka = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("Vokzalwav");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(1,1,1).p("AISAAQAACyibB+QibB+jcAAQjbAAibh+Qibh+AAiyQAAixCbh+QCbh+DbAAQDcAACbB+QCbB+AACxg");
	this.shape.setTransform(0.5,0);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#CCCCCC").s().p("Al2EwQibh+AAiyQAAixCbh+QCbh+DbAAQDbAACcB+QCbB+AACxQAACyibB+QicB+jbAAQjbAAibh+g");
	this.shape_1.setTransform(0.5,0);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[]}).to({state:[{t:this.shape_1},{t:this.shape}]},3).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-53.5,-44,108,88);


(lib.DomPravo = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("DonPraviwav");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(1,1,1).p("ASMAAQAAGllVEqQlVEqniAAQnhAAlVkqQlVkqAAmlQAAmkFVkqQFVkqHhAAQHiAAFVEqQFVEqAAGkg");
	this.shape.setTransform(-0.9,9.8);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#CCCCCC").s().p("As2LOQlVkpAAmlQAAmkFVkqQFVkqHhAAQHiAAFVEqQFUEqABGkQgBGllUEpQlVEqniAAQnhAAlVkqg");
	this.shape_1.setTransform(-0.9,9.8);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[]}).to({state:[{t:this.shape_1},{t:this.shape}]},3).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-118.2,-92.8,234.7,205.3);


(lib.DinamoKnopka = function(mode,startPosition,loop,reversed) {
if (loop == null) { loop = true; }
if (reversed == null) { reversed = false; }
	var props = new Object();
	props.mode = mode;
	props.startPosition = startPosition;
	props.labels = {};
	props.loop = loop;
	props.reversed = reversed;
	cjs.MovieClip.apply(this,[props]);

	// timeline functions:
	this.frame_2 = function() {
		playSound("Dinamowav");
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).wait(2).call(this.frame_2).wait(2));

	// Слой_1
	this.shape = new cjs.Shape();
	this.shape.graphics.f().s("#000000").ss(1,1,1).p("AGQAAQAACDh1BcQh1BcimAAQilAAh1hcQh1hcAAiDQAAiCB1hcQB1hcClAAQCmAAB1BcQB1BcAACCg");
	this.shape.setTransform(3,-0.45);

	this.shape_1 = new cjs.Shape();
	this.shape_1.graphics.f("#CCCCCC").s().p("AkaDeQh1hcAAiCQAAiCB1hcQB2hcCkAAQCmAAB0BcQB2BcAACCQAACCh2BcQh0BdimAAQikAAh2hdg");
	this.shape_1.setTransform(3,-0.45);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[]}).to({state:[{t:this.shape_1},{t:this.shape}]},3).wait(1));

	this._renderFirstFrame();

}).prototype = p = new cjs.MovieClip();
p.nominalBounds = new cjs.Rectangle(-38,-32.9,82,65);


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

	this.actionFrames = [0];
	this.isSingleFrame = false;
	// timeline functions:
	this.frame_0 = function() {
		if(this.isSingleFrame) {
			return;
		}
		if(this.totalFrames == 1) {
			this.isSingleFrame = true;
		}
		this.DomPravo.addEventListener("click", () => {
		    createjs.Sound.play("DomPravi");
		});
		
		this.DinamoKnopka.addEventListener("click", () => {
		    createjs.Sound.play("Dinamo");
		});
		
		this.VokzalKnopka.addEventListener("click", () => {
		    createjs.Sound.play("Vokzal");
		});
	}

	// actions tween:
	this.timeline.addTween(cjs.Tween.get(this).call(this.frame_0).wait(1));

	// Слой_1
	this.VokzalKnopka = new lib.VokzalKnopka();
	this.VokzalKnopka.name = "VokzalKnopka";
	this.VokzalKnopka.setTransform(716.05,909.85,1.5028,1.6814,0,0,0,0,0.1);
	new cjs.ButtonHelper(this.VokzalKnopka, 0, 1, 2, false, new lib.VokzalKnopka(), 3);

	this.DinamoKnopka = new lib.DinamoKnopka();
	this.DinamoKnopka.name = "DinamoKnopka";
	this.DinamoKnopka.setTransform(1434.95,543.35,2.4791,2.5749);
	new cjs.ButtonHelper(this.DinamoKnopka, 0, 1, 2, false, new lib.DinamoKnopka(), 3);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.DinamoKnopka},{t:this.VokzalKnopka}]}).wait(1));

	// Слой_2
	this.DomPravo = new lib.DomPravo();
	this.DomPravo.name = "DomPravo";
	this.DomPravo.setTransform(235.7,430.4);
	new cjs.ButtonHelper(this.DomPravo, 0, 1, 2, false, new lib.DomPravo(), 3);

	this.instance = new lib.Растровоеизображение3();
	this.instance.setTransform(-63,0,2.6868,1.8765);

	this.timeline.addTween(cjs.Tween.get({}).to({state:[{t:this.instance},{t:this.DomPravo}]}).wait(1));

	this._renderFirstFrame();

}).prototype = p = new lib.AnMovieClip();
p.nominalBounds = new cjs.Rectangle(897,540,1106.1,539);
// library properties:
lib.properties = {
	id: '60D8ABEBEF192C4FA06814645A5ADB2B',
	width: 1920,
	height: 1080,
	fps: 30,
	color: "#FFFFFF",
	opacity: 1.00,
	manifest: [
		{src:"images/lab03_HTML5 Canvas_atlas_1.png?1758691094281", id:"lab03_HTML5 Canvas_atlas_1"},
		{src:"sounds/Dinamowav.mp3?1758691094292", id:"Dinamowav"},
		{src:"sounds/DonPraviwav.mp3?1758691094292", id:"DonPraviwav"},
		{src:"sounds/Vokzalwav.mp3?1758691094292", id:"Vokzalwav"}
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
an.compositions['60D8ABEBEF192C4FA06814645A5ADB2B'] = {
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