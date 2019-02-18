<template>
    <div v-show="showResultFrom" class="result-filter-text">
        &nbsp;{{resultStringPrefix}}&nbsp;<span class="result-filter-text-bold">{{resultStringGuide}}</span>
    </div>
</template>

<script>
import { LanguageService } from "../../../../services/language/languageService";
export default {
    data(){
        return{
            dictionary:{
                classResult: LanguageService.getValueByKey("resultFilter_class_result"),
                universityResult: LanguageService.getValueByKey("resultFilter_university_result"),
                spitballResult: LanguageService.getValueByKey("resultFilter_spitball_result")
            }
        }
    },
    computed:{
        resultStringGuide(){
                if(!!this.$route.query.term){
                    if(!!this.$route.query && !!this.$route.query.Course){
                        return this.$route.query.Course
                    }else if(!!this.$route.query && !!this.$route.query.uni){
                        return this.$route.query.uni;
                    }else{
                        return "";
                    }
                }else{
                    return "";
                }
            },
            resultStringPrefix(){
                if(!!this.$route.query.term){
                    if(!!this.$route.query && !!this.$route.query.Course){
                        return this.dictionary.classResult;
                    }else if(!!this.$route.query && !!this.$route.query.uni){
                        return this.dictionary.universityResult;
                    }else{
                        return this.dictionary.spitballResult;
                    }
                }else{
                    return "";
                }
            },
            showResultFrom(){
                return !!this.$route.query.term;
            },
    }
}
</script>

<style>

</style>
