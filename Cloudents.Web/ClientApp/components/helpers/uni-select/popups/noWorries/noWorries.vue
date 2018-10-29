<template>
    <div class="noWorries-Container">
        <div class="close-btn-container">
            <v-icon @click="stay()">sbf-close</v-icon>
        </div>
        <div class="title-container">
            <span>:)  No Worries</span>
        </div>
        <div class="explain-container">
            <span>You can select school & classes at any time from your user menu.</span>
        </div>
        <div class="img-container"></div>
        <div class="footer-container">
            <span @click="stay()">Select School now</span>
            <span @click="leave()" class="clear">Got It!</span>
        </div>
    </div>
</template>

<script>
export default {
    props:{
        fnLeave:{
            required: true  
        },
        fnStay:{
            required: true  
        }
    },
    methods:{
        stay(){
            this.fnStay();
        },
        leave(){
             this.fnLeave();
        }
    }
}
</script>

<style lang="less" src="./noWorries.less"></style>
