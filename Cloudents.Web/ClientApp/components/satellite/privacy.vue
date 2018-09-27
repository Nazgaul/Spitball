<template>
    <div class="term" v-html="privacyHtml"></div>
</template>
<script>
import { LanguageService } from '../../services/language/languageService'
export default {
    data(){
        return {
            privacyHtml: LanguageService.getPrivacyPage()
        }
    }
}
</script>
<style lang="less" src="./term.less">
</style>