<template>
        <div class="select-university-container">
            <div class="title-container">
                Set your school & classes
            </div>
            <div class="explain-container">
                By setting this up you will get the best questions & study info fit for your shool & classes
            </div>
            
            <div class="select-school-container">
                <input type="text" placeholder="Select Your School" @click="inputClicked">  
                <v-icon>sbf-arrow-right</v-icon>
            </div>

            <div class="skip-container">
                <a @click="skipUniSelect()">Skip for now</a> 
            </div>
        </div>
</template>

<script>
import { mapGetters, mapActions } from 'vuex';
export default {
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
        ...mapActions(['changeSelectUniState', 'setUniversityPopStorage_session']),
        inputClicked(){
            this.fnMethods.changeStep(this.enumSteps.set_school)
        },
        skipUniSelect(){
            // this.setUniversityPopStorage_session();
            // this.changeSelectUniState(false);
            this.fnMethods.openNoWorriesPopup();
        }
    }
}
</script>

<style>

</style>
