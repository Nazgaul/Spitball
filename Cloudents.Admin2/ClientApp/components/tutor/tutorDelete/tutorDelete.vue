<template>
    <div class="tutorDelete">
        <h1 align="center">Delete Tutuor by Id</h1>
        <v-layout align-center justify-center column fill-height>
        <div class="input-container">
            <v-text-field solo class="user-input-text" v-model="tutorId" placeholder="Insert document id..."/>
        </div>
        <div class="delete-button-container">
            <v-btn :loading="loading" color="red" :disabled="lock" @click.prevent="deleteTutoring(tutorId)">Delete</v-btn>
        </div>
    </v-layout>
    </div>
</template>

<script>
import { deleteTutor } from './tutorDeleteService.js'

export default {
    data() {
        return {
            lock: false,
            loading: false,
            tutorId: ''
        }
    },
    methods: {
        deleteTutoring(id) {
            this.loading = true;
            if(!id) return
            deleteTutor(id).then(resp => {
                this.tutorId = "";
                this.$toaster.success('Tutor Deleted');
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
<style lang="less">
    .tutorDelete{
        width: 100%;
    }
</style>