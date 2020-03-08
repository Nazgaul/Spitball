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
        <div v-show="showUrl">{{uploadedUrl}}</div>
        </v-layout>
   </div>
</template>


<script>
import FileUpload from 'vue-upload-component'
export default {
  components: {
    FileUpload,
  },
  data() {
    return {
      files: [],
      // 1MB by default
      chunkMinSize: 0,
      chunkMaxActive: 3,
      chunkMaxRetries: 5,
      uploadedUrl: '',
      showUrl: false
    }
  },
  methods: {
    inputFile(newFile, oldFile) {
      if (newFile && !oldFile) {
        // add
        console.log('add', newFile)
        this.$refs.upload.active = true
      }
      if (newFile && oldFile) {
        // update
        debugger
        if(newFile.response.hasOwnProperty("url")) {
        this.uploadedUrl = newFile.response.url
        this.showUrl = true
        }
        console.log('update', newFile)
      }
      if (!newFile && oldFile) {
        // remove
        console.log('remove', oldFile)
      }
    }
  }
}
</script>

