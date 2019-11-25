<template>
    <span class="change-course-container" @click="openEditClass()" v-if="singleFilter.id.toLowerCase() === 'course'"><span class="edit-after-icon" v-language:inner>menuList_Change</span><v-icon class="edit-after-icon">sbf-edit-icon</v-icon></span>
</template>

<script>
import { mapActions, mapMutations, mapGetters } from 'vuex'
export default {
    methods:{
        ...mapGetters(['getAllSteps']),
        openEditClass(){
           this.$router.push({name: 'addCourse'})
        },
        
    }
}
</script>

<style>

</style>
