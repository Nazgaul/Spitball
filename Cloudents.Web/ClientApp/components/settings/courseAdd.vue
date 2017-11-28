<template>
    <div>
        <form @submit.prevent="$_submit" @keydown.enter.prevent="$_submit">
           <v-container fluid grid-list-l> <v-layout row justify-center>
                <v-flex xs4><v-text-field dark color="white" v-model.trim="name" required placeholder="Course Name" />
                </v-flex>
            </v-layout>
            <v-layout row justify-center>
                <v-flex xs4>
                    <v-text-field dark color="white" v-model.trim="code" placeholder="Course Code(optional)" /></v-flex>
            </v-layout></v-container>
            <v-btn @click="$_submit"
                   :disabled="!name">
                submit
            </v-btn>
        </form>
    </div>
  
</template>
<script>
    import {mapActions} from 'vuex'
    export default {
        data() {
            return {
                name:'',code:''
            }
        },
        methods: {
            ...mapActions(['createCourse']),
            $_submit() {
                this.createCourse({ name: this.name, code: this.code }).then((obj) => {
                    this.$emit('done',obj)
                })
            }
        }
    }
</script>