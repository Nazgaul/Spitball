<template>
    <div class="noWorries-Container">
        <div class="close-btn-container">
            <v-icon @click="stay()">sbf-close</v-icon>
        </div>
        <div class="title-container">
            <span v-language:inner>uniSelect_no_worries</span>
        </div>
        <div class="explain-container">
            <span v-language:inner>uniSelect_select_school_any_time</span>
        </div>
        <div class="img-container"></div>
        <div class="footer-container">
            <span @click="stay()" class="bold-text" v-language:inner>uniSelect_select_school_now</span>
            <span @click="leave()" class="clear" v-language:inner>uniSelect_got_it</span>
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
