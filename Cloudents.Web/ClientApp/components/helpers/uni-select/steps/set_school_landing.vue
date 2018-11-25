<template>
        <!--<div class="select-university-container">-->
    <div>
            <div class="title-container" v-language:inner>uniSelect_set_school_class</div>
            <div class="explain-container" v-language:inner>uniSelect_set_school_class_explain</div>
            <div class="select-school-container">

                <input type="text" style="padding-left:16px;" placeholder="uniSelect_select_school_placeholder" v-language:placeholder @click="inputClicked">  
                <v-icon :class="{'rtl': isRtl}">sbf-arrow-right</v-icon>
            </div>

            <div class="skip-container">
                <a @click="skipUniSelect()" v-language:inner>uniSelect_skip_for_now</a> 
            </div>
        </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
    data(){
        return{
            isRtl: global.isRtl
        }
    },
    props:{
        fnMethods:{
            required:true,
            type:Object
        },
        enumSteps:{
            required:true,
            type:Object
        }
    },

    methods:{
        inputClicked(){
            this.fnMethods.changeStep(this.enumSteps.set_school)
        },
        skipUniSelect(){
            this.fnMethods.openNoWorriesPopup();
        }
    }
}
</script>

<style>

</style>
