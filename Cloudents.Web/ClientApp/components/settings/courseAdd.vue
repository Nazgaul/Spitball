<template>
    <div>
        <h5 class="text-xs-center">Add Course</h5>
        <form @submit.prevent="$_submit" @keydown.enter.prevent="$_submit">
            <v-text-field v-model.trim="name" required placeholder="Course Name" />
            <v-text-field v-model.trim="code" placeholder="CourseCode" />
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