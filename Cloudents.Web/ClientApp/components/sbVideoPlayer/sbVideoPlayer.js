export default {
    data() {
        return {
            isReady:false,
            myPlayer: ''
        }
    },
    props:{
        id:{
            type: String,
            default:'azuremediaplayer'
        },
        poster:{
            type: String,
            default:''
        },
        controls:{
            default: true
        },
        autoplay:{
            type: Boolean,
            default: false
        },
        width:{
            default: '600'
        },
        height:{
            default: '400'
        },
        tabindex:{
            default: '0'
        },
        dataSetup:{
            default: '{"logo":{"enabled":false},"plugins":{ "contentTitle": {"name": "Azure Medi Services Overview"}}}'
        },
        src:{
            type: String,
            required: true,
        },
        type:{
            type: String,
            default: 'application/vnd.ms-sstr+xml'
        },
        isResponsive:{
            type: Boolean,
            default: false
        },
        title:{
            type: String
        }
    },
    computed: {
        uniqueID(){
            return `${this.id}_${Math.random().toString(36).substr(2, 9)}`
        }
    },
    watch:{
        src(newVal, oldVal){
            console.log(newVal);
            if(!!this.myPlayer){
                let srcObj = {
                    src: newVal,
                    type: this.type
                }
                this.myPlayer.src([srcObj]);
            }
        }
    },
    methods: {
        initVideoPlayer(){
            let videoOptions = {
                // "fluid": true,
                "nativeControlsForTouch": false,
                "logo":{"enabled":false},
                "techOrder": ["azureHtml5JS", "flashSS", "html5FairPlayHLS","silverlightSS", "html5"], 
                controls: this.controls,
                autoplay: this.autoplay,
                "poster": this.poster,
                width: this.width,
                height: this.height,
                "language":global.lang,
                // traceConfig: {
                //     TraceTargets: [{ target: 'console' }],
                //     maxLogLevel: 3
                // },
            }
            let uniqueID = this.uniqueID
            let srcObj = {src:this.src,type:this.type}
            this.myPlayer = amp(uniqueID,videoOptions);
            this.myPlayer.src([srcObj]);
            this.myPlayer.addEventListener('ended',(e)=>{
                this.$emit('videoEnded')
            })
        },
        loadStyle(){
            return new Promise((resolve, reject) => {
                if (document.querySelector('#amp-azure')) return resolve()
                let linkTag = document.createElement('link')
                linkTag.id = '#amp-azure'
                linkTag.rel = 'stylesheet'
                linkTag.href = `//amp.azure.net/libs/amp/2.3.1/skins/amp-flush/azuremediaplayer.min.css`
                document.head.insertBefore(linkTag, document.head.firstChild)
                return resolve()
            })
        }
    },
    beforeCreate() {
        let self = this
        let ampUrl = '//amp.azure.net/libs/amp/2.3.1/azuremediaplayer.min.js'
        this.$loadScript(ampUrl).then(()=>{
            self.loadStyle().then(()=>{
                self.initVideoPlayer()
            })
        })
    },
}
