export default {
    data() {
        return {
            isReady:false,
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
        posterPlaceHolder(){
            // need to change it: img from RAM by size;
            return `https://miro.medium.com/max/${this.width}/1*MPHVp5hi-uSwYYOwlXFuzw.png`
        }
    },
    methods: {
        initVideoPlayer(){
            // data-setup='{plugins: { "contentTitle": {"name": "Azure Medi Services Overview"}}}'...>
            let videoOptions = {
                "fluid": true,
                "nativeControlsForTouch": false,
                "logo":{"enabled":false},
                "plugins": { 
                    "titleOverlay": { 
                        "name": this.title, 
                        "horizontalPosition": "left", 
                        "verticalPosition": "center" 
                    } 
                },
                "techOrder": ["azureHtml5JS", "flashSS", "html5FairPlayHLS","silverlightSS", "html5"], 
                // ...this.dataSetup,
                controls:this.controls,
                autoplay:this.autoplay,
                width:this.width,
                height:this.height
            }
            let srcObj = {src:this.src,type:this.type}
            let myPlayer = amp(this.id,videoOptions);
            myPlayer.src([srcObj]);
        }
    },
    beforeCreate() {
        let self = this
        let ampUrl = '//amp.azure.net/libs/amp/2.1.5/azuremediaplayer.min.js'
        this.$loadScript(ampUrl).then(()=>{
            self.$loadScript('//azure-samples.github.io/media-services-javascript-azure-media-player-title-overlay-plugin/amp-titleOverlay.js').then(()=>{
                self.initVideoPlayer()
                })
        })
    },
}
