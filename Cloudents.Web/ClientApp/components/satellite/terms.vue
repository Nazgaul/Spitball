<template>
    <div class="term" v-html="termsHtml"></div>
</template>
<script>
import { LanguageService } from '../../services/language/languageService'
export default {
    data(){
        return {
            termsHtml: LanguageService.getTermPage()
        }
    }
}
</script>

<style lang="less" src="./term.less"></style>