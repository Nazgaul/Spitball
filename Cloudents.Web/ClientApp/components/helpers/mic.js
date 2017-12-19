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
            console.log("here")
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
                this.recognition.onresult = function (event) {
                    _self.msg = event.results[0][0].transcript;
                    _self.isRecording = false;
                };
            }
        },
    
        computed: {
            voiceEnable() { return ("webkitSpeechRecognition" in window) || ("SpeechRecognition" in window)},
            voiceAppend(){return this.voiceEnable?(this.isRecording?'sbf-mic-recording':'sbf-mic'):''}
        }
    };