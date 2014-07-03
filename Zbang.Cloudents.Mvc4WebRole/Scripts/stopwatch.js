define('stopwatch',[], function() {
    var Stopwatch = function(output, options) {

        //var timer       = createTimer(),
        var that = this,
            offset,
            clock,
            output,
            interval;

        // default options
        options = options || {};
        options.delay = options.delay || 250;

        if (!output) {
            throw 'No output provided';
        }

        // initialize
        reset();

        function start() {
            if (!interval) {
                offset = Date.now();
                that.startTime = new Date();
                interval = setInterval(update, options.delay);
                that.isRunning = true;
            }
        }

        function stop() {
            if (interval) {
                that.endTime = new Date();
                clearInterval(interval);
                interval = null;
                that.isRunning = false;
            }
        }

        function reset() {
            that.startTime = null;
            clock = 0;
            render();
        }

        function update() {
            clock += delta();
            render();
        }

        function render() {
            var seconds = Math.floor(seconds = (clock / 1000) % 60),
                minutes = Math.floor((clock / (1000 * 60)) % 60),
                hours = Math.floor((clock / (1000 * 60 * 60)) % 24);
            that.lastTime = ((hours <= 9) ? '0' + hours : hours) + ":" + ((minutes <= 9) ? '0' + minutes : minutes) + ':' + ((seconds <= 9) ? '0' + seconds : seconds);
            output.textContent = that.lastTime;
        }

        function delta() {
            var now = Date.now(),
                d = now - offset;

            offset = now;
            return d;
        }

        function renderTimeDiff() {
            var diff = that.endTime - that.startTime;
            var seconds = Math.floor(seconds = (diff / 1000) % 60),
                minutes = Math.floor((diff / (1000 * 60)) % 60),
                hours = Math.floor((diff / (1000 * 60 * 60)) % 24);
            that.lastTime = ((hours <= 9) ? '0' + hours : hours) + ":" + ((minutes <= 9) ? '0' + minutes : minutes) + ':' + ((seconds <= 9) ? '0' + seconds : seconds);
            output.textContent = that.lastTime;
        }

        // public API
        that.start = start;
        that.stop = stop;
        that.reset = reset;
        that.renderTimeDiff = renderTimeDiff;
        that.isRunning = false;
    };
    return Stopwatch;
}())