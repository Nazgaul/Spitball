<template>


<div>
    <h1 align="center">Upload Files</h1>
     <v-layout align-center justify-center column fill-height>
        <file-upload
          class="btn btn-primary"
          post-action="api/adminUpload/upload"
          
          :chunk-enabled="true"
          :chunk="{
            action: 'api/adminUpload/upload',
            minSize: chunkMinSize,
            maxActive: chunkMaxActive,
            maxRetries: chunkMaxRetries
          }"
          extensions="gif,jpg,jpeg,png,webp,mp4"
          accept="image/png,image/gif,image/jpeg,image/webp,video/mp4"
          :size="1024 * 1024 * 10"
          v-model="files"
          @input-file="inputFile"
          ref="upload">
          <v-btn color="blue">Select files</v-btn>
        </file-upload>

        <div class="d-flex justify-center align-center" v-if="loader">
                        <div class="text-xs-center">
                        <v-progress-circular :size="100"
                                            :width="5"
                                             color="primary"
                                             indeterminate>
                            Uploading...
                        </v-progress-circular>
                        </div>
                    </div>
        <!-- <div v-show="showUrl">{{uploadedUrl}}</div> -->
        <ul>
          <li v-for="item in uploadedUrl" :key="item">
            {{ item }}
          </li>
        </ul>
        </v-layout>
   </div>
</template>


<script>
import FileUpload from 'vue-upload-component'
import { getBlobs } from './uploadService.js'

export default {
  components: {
    FileUpload,
  },
  data() {
    return {
      files: [],
      // 0MB by default
      chunkMinSize: 0,
      chunkMaxActive: 3,
      chunkMaxRetries: 5,
      uploadedUrl: [],
      showUrl: false,
      loader : false
    }
  },
  methods: {
    inputFile(newFile, oldFile) {
      if (newFile && !oldFile) {
        // add
        console.log('add', newFile)
        this.$refs.upload.active = true
        this.loader = true
      }
      if (newFile && oldFile) {
        // update
        if(newFile.response.hasOwnProperty("url")) {
        this.uploadedUrl.unshift(newFile.response.url)
        this.showUrl = true
        this.loader = false
        }
        console.log('update', newFile)
      }
      if (!newFile && oldFile) {
        // remove
        console.log('remove', oldFile)
      }
    }
  },
  created() {
    getBlobs().then((list) => {
      this.uploadedUrl = list
    })
  }
}
</script>

