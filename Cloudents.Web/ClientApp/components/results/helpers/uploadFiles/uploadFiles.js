import { mapGetters, mapActions } from 'vuex';
import sbDialog from '../../../wrappers/sb-dialog/sb-dialog.vue';
import Vue from 'vue';
import FileUpload from 'vue-upload-component/src';
import googlePickerApi from './initApi.js'
//var VueUploadComponent = import('vue-upload-component');
Vue.component('file-upload', FileUpload);


export default {
    components: {
        sbDialog,
        FileUpload

    },
    name: "uploadFiles",

    data() {
        return {
            showUploadDialog: false,
            uploadUrl: "/api/upload/ask",
            counter: 0,
            uploadsPreviewList: [],
            dbReady: false,
            regularUploadFiles: [],
            progressValue: '',
            files: []


        }
    },
    props: {},
    computed: {
        ...mapGetters({
            accountUser: 'accountUser',
        }),
        progressShow() {
            if (this.progressValue === '100.00') {
                return false
            }

        }

    },
    watch: {
        files(newValue, oldValue) {
            console.log('newVal::: ',newValue, 'files',this.files)
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
            if (isDbExists) {
                return
            }
            //if didnt exist before
            let dbjs = document.createElement('script');
            dbjs.id = "dropboxjs";
            dbjs.async = false;
            dbjs.setAttribute('data-app-key', 'mii3jtxg6616y9g');
            dbjs.src = "https://www.dropbox.com/static/api/1/dropins.js";
            document.getElementsByTagName('head')[0].appendChild(dbjs);
            dbjs.onload = () => {
                this.dbReady = true; // enable dropbox upload btn when script is ready
            }
        },

        DbFilesList() {
            var self = this;
            let options = {
                success:  (files)=> {
                    let type = 'dropBox'
                    files.forEach((file)=> {
                        self.uploadsPreviewList.push({
                            name : file,
                            link:  file.thumbnails["64x64"],
                            type: type
                        });
                        this.files.push({
                            name: file.name,
                            link: file.link,
                            size: file.bytes,
                            type: type

                        });
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
        },

        //load google picker uploader
        loadGooglePicker() {
            googlePickerApi.loadPicker(this.returnedGPickerData)
        },

        returnedGPickerData(data) {
            if (data.action == google.picker.Action.PICKED) {
               // let files = data.docs;
                let doc, url, name, size;
                let type = "googleDrive";
                for (var i = 0, l = data[google.picker.Response.DOCUMENTS].length; i < l; i++) {
                    doc = data[google.picker.Response.DOCUMENTS][i];
                    if (doc.type === google.picker.Type.DOCUMENT) {
                        url = doc[google.picker.Document.URL];
                    } else {
                        url = 'https://drive.google.com/uc?id=' + doc.id;
                    }
                    name = doc.name;
                    size = doc.sizeBytes;
                    if (!url) {
                        continue;
                    }
                    this.files.push({
                        name: name,
                        link: url,
                        size: size,
                        type: type

                    });
                    this.uploadsPreviewList.push({
                        name: name,
                        link: 'https://az32006.vo.msecnd.net:443/spitball/quizlet.png',
                        type: type
                    })
                }


            }

        },
        // regular upload methods
        inputFile(newFile, oldFile) {
            if (newFile && !oldFile) {
                // Add file
            }
            // Upload progress
            if (newFile.progress) {
                this.progressValue = newFile.progress;
                console.log('progress', newFile.progress, newFile)
            }
            // Upload error
            if (newFile && oldFile && newFile.error !== oldFile.error) {
                console.log('error', newFile.error, newFile)
            }
            // Get response data
            if (newFile && oldFile && !newFile.active && oldFile.active) {
                // Get response data
                console.log('response', newFile.response);
                if (newFile.xhr) {
                    //  Get the response status code
                    console.log('status', newFile.xhr.status)
                }
            }
        },
        inputFilter(newFile, oldFile, prevent) {
            if (newFile && !oldFile) {
                // Add file
                // Filter non-image file remove for docs
                // Will not be added to files

                if (!/\.(jpeg|jpe|jpg|gif|png|webp)$/i.test(newFile.name)) {
                    return prevent()
                }

                // Create the 'blob' field for thumbnail preview
                newFile.blob = ''
                let URL = window.URL || window.webkitURL;
                let type = 'fromDisk'
                if (URL && URL.createObjectURL) {
                    this.uploadsPreviewList.push({
                        name: newFile.name,
                        link: URL.createObjectURL(newFile.file),
                        type: type
                    })

                }
            }
            if (newFile && oldFile) {
                // Update file
                // Increase the version number
                if (!newFile.version) {
                    newFile.version = 0
                }
                newFile.version++
            }
            if (!newFile && oldFile) {
                // Remove file
                // Refused to remove the file
                // return prevent()
            }
        },
        deletePreview: function(index){
            this.uploadsPreviewList.splice(index, 1);
            this.files.splice(index, 1)
        },
    },

    mounted() {

    },

    created() {
    }

}