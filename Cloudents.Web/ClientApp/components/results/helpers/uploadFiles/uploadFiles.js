import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';

var Uploader = require('html5-uploader');


export default {
    components: {
        sbDialog
    },
    name: "uploadFiles",

    data() {
        return {
            showUploadDialog: false,
            uploadUrl: "/api/upload/ask",
            counter: 0,
            uploadsPreviewList: [],
            accessToken: 'Gwiu4ufC-iAAAAAAAAAAF_y9wc13EuwRyUDc6EtnHSnbLq7_Hb7Y9hzPUVlSXQyM',
            dbReady: false,
            uploadProgress: 0

        }
    },
    props: {},
    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
        }),
        dynamicProgress(){
            return this.uploadProgress
        }
    },
    methods: {
        ...mapActions(["updateLoginDialogState", 'updateUserProfileData']),
        openUploaderDialog() {
            if (this.accountUser == null) {
                this.updateLoginDialogState(true);
                //set user profile
                this.updateUserProfileData('profileHWH')
            } else {
                this.loadDropBoxSrc(); // load Drop box script
                this.showUploadDialog = true;
            }
        },
        loadDropBoxSrc() {
            // if exists prevent duplicate loading
            let isDbExists = !!document.getElementById('dropboxjs');
            if(isDbExists){
                return
            }
            //if didnt exist before
            let dbjs = document.createElement('script');
            dbjs.id = "dropboxjs";
            dbjs.async = false;
            dbjs.setAttribute('data-app-key', 'mii3jtxg6616y9g');
            dbjs.src = "https://www.dropbox.com/static/api/1/dropins.js";
            document.getElementsByTagName('head')[0].appendChild(dbjs);
            dbjs.onload = ()=> {
                this.dbReady = true; // enable dropbox upload btn when script is ready
            }
        },

    DbFilesList() {
        var self = this;
        let options = {
            success: function (files) {
                files.forEach(function (file) {
                    self.uploadsPreviewList.push(file.thumbnails["64x64"]);
                });
            },
            cancel: function () {
                //optional
            },
            linkType: "preview", // "preview" or "direct"
            multiselect: true, // true or false
            extensions: ['.png', '.jpg', 'doc', 'pdf'],
        };
        global.Dropbox.choose(options);
    }
},



mounted()

{
    //Used html5-uploader, examples here: http://mpangrazzi.github.io/
    var self = this;
    var multiple = new Uploader({
        el: '#file-input',
        url: this.uploadUrl
    });

    multiple.on('files:added', function (val) {
        console.log('size',val, val.size);
        //this.files = val.filter(i => i.type.indexOf("image") > -1);
        this.files = val;
        this.upload()
    });

    multiple.on('file:preview', function (file, $img) {
        let files = this.getFiles();
        if ($img && files.length) {
            self.counter = self.counter + 1;
            self.uploadsPreviewList.push($img.src);

        }
    });

    multiple.on('upload:done', function (response) {
        self.$emit('addFile', JSON.parse(response).files.toString());
    });
    multiple.on('upload:progress', function (progress) {
        self.uploadProgress = progress;
        console.log('progress: %s', progress);
    });

},

created(){

}

}