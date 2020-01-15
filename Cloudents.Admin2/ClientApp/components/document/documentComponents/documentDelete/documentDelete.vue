<template>
    <div>
        <h1 align="center">Delete Document by Id</h1>
        <v-layout align-center justify-center column fill-height>
        <div class="input-container">
            <v-text-field solo class="user-input-text" v-model="documentIdString" placeholder="example: 1245,6689,1123"/>
        </div>
        <div class="delete-button-container">
            <v-btn :loading="loading" color="red" :disabled="lock" @click.prevent="deleteDocuments">Delete</v-btn>
        </div>
    </v-layout>
    </div>
</template>

<script>
import {deleteDocument} from './documentDeleteService.js'
export default {
    data(){
        return{
            documentIds: [],
            documentIdString: '',
            lock: false,
            loading: false,
        }
    },
    methods:{
        deleteDocuments(){
                this.loading = true;
                if (this.documentIdString.length > 0) {
                    this.documentIds = this.documentIdString.split(',');
                    let numberArr = [];
                    this.documentIds.forEach(id => {
                        let num = parseInt(id.trim());
                        if (!!num) {
                            return numberArr.push(num);
                        }

                    })
                    deleteDocument(numberArr)
                        .then(resp => {

                                this.$toaster.success(`Documents were deleted: ${this.documentIdString}`);
                                this.documentIdString = '';
                                this.documentIds = [];
                                this.loading = false;


                            },
                            (error) => {

                                this.$toaster.error('Something went wrong');
                                this.loading = false;
                            }
                        )
                }
            }
    }
}
</script>

<style lang="less" scoped>
    .input-container{
        width:250px;
        margin: 0 auto;
        margin-top: 20px; 
    }
    .delete-button-container{
        margin-top: 15px;
    }
</style>
