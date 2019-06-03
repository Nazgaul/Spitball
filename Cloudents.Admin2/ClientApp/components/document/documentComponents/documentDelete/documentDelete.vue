<template>
    <div>
        <h1>Delete Document by Id</h1>
        <v-layout align-center justify-center column fill-height>
        <div class="input-container">
            <v-text-field solo class="user-input-text" v-model="documentId" placeholder="Insert document id..."/>
        </div>
        <div class="delete-button-container">
            <v-btn :loading="loading" color="red" :disabled="lock" @click.prevent="deleteDocument(documentId)">Delete</v-btn>
        </div>
    </v-layout>
    </div>
</template>

<script>
import {deleteDocument} from './documentDeleteService.js'
export default {
    data(){
        return{
            documentId: "",
            lock: false,
            loading: false,
        }
    },
    methods:{
        deleteDocument(id){
            this.loading = true;
            if(!id) return;
            this.lock = true;
            deleteDocument(id).then(resp => {
                this.documentId = "";
                this.$toaster.success('Document Deleted');
                    this.loading = false;
            },
            (error) => {
                this.$toaster.error('Something went wrong');
                this.loading = false;

            }).finally(()=>{
                this.lock = false;
                this.loading = false;

            })

        }
    }
}
</script>

<style lang="scss" scoped>
    .input-container{
        width:250px;
        margin: 0 auto;
        margin-top: 20px; 
    }
    .delete-button-container{
        margin-top: 15px;
    }
</style>
