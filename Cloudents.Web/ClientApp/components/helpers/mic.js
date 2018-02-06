export const micMixin = {    
        data() {
            return {
                isRecording: false,
                recognition: false,
                msg:""
            };
        },
    
        methods: {
            $_voiceDetection() {
                if(!this.isRecording) {
                    this.recognition.start();
                }else{
                    this.isRecording=false;
                    this.recognition.stop();
                }
            }
        },

        created() {
            if (this.voiceEnable) {
                let SpeechRecognition = SpeechRecognition || webkitSpeechRecognition;
                this.recognition = new SpeechRecognition();
                this.recognition.lang = "en-US";
                this.recognition.interimResults = false;
                this.recognition.maxAlternatives = 5;
                let _self = this;
    
                this.recognition.onstart = function () {
                    _self.isRecording = true;
                };
                this.recognition.onerror = function() {
                    console.log('error');
                    this.isRecording = false;};
                this.recognition.onend = function() {
                    console.log('end')
                    this.isRecording = false;};
                this.recognition.onresult = function (event) {
                    _self.msg = event.results[0][0].transcript;
                    this.$nextTick(()=>{
                        this.$ga.event('Search','Voice', _self.msg);
                        _self.isRecording = false;
                        if(_self.$vuetify.breakpoint.smAndDown){
                            _self.submitMic();
                        }
                    });
                };
            }
        },
    
        computed: {
            voiceEnable() { return ("webkitSpeechRecognition" in window) || ("SpeechRecognition" in window)},
            voiceAppend(){return this.voiceEnable?(this.isRecording?'sbf-mic-recording':'sbf-mic'):''}
        },
    props:{submitFunc:{type:Function}}
    };