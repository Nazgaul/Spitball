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
                    _self.isRecording = false;};
                this.recognition.onend = function() {
                    console.log('end')
                    _self.isRecording = false;};
                this.recognition.onresult = function (event) {
                    let msg=event.results[0][0].transcript;
                    _self.msg = msg;
                    _self.$ga.event('Search','Voice', msg);
                    _self.isRecording = false;
                    if(_self.$vuetify.breakpoint.smAndDown){
                        _self.submitMic(msg);
                    }
                };
            }
        },
    
        computed: {
            voiceEnable() { return ("webkitSpeechRecognition" in window) || ("SpeechRecognition" in window)},
        },
    props:{submitFunc:{type:Function}}
    };