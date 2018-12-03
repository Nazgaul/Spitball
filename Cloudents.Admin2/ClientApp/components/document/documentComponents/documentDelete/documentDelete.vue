<template>
    <div>
        <h1>Delete Document by Id</h1>
        <div class="input-container">
            <v-text-field solo class="user-input-text" v-model="documentId" placeholder="Insert document id..."/>
        </div>
        <div class="delete-button-container">
            <v-btn round color="red" :disabled="lock" @click.prevent="deleteDocument(documentId)">Delete</v-btn>
        </div>
    </div>
</template>

<script>
import {deleteDocument} from './documentDeleteService.js'
export default {
    data(){
        return{
            documentId: "",
            lock: false,
        }
    },
    methods:{
        deleteDocument(id){
            if(!id) return;
            this.lock = true;
            deleteDocument(id).then(resp => {
                this.documentId = "";
                this.$toaster.success('Document Deleted');
            },
            (error) => {
                this.$toaster.error('Something went wrong');
            }).finally(()=>{
                this.lock = false;
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
