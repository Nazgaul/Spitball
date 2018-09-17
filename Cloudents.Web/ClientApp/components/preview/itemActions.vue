<template>
    <div class="actions">
        <v-tooltip bottom v-if="showActions">
            <info-icon slot="activator" class="info-icon ml-4"></info-icon>
            <span>itemAction_policy</span>
        </v-tooltip>
        <a target="_blank" :href="$route.path+'/download'" v-if="showActions" class="ml-4">
            <download-icon></download-icon>
        </a>
        <a href="#" @click.prevent="$_back" class="ml-4">
            <close-icon></close-icon>
        </a>
    </div>
</template>
<script>
    import { LanguageService } from "../../services/language/languageService";
    import closeIcon from './svg/close-icon.svg'
    import downloadIcon from './svg/download-icon.svg'
    import infoIcon from './svg/info-icon.svg'
    import printIcon from './svg/print-icon.svg'
    import moreIcon from './svg/more-icon.svg'

    export default {
        components: {
            closeIcon, downloadIcon, infoIcon, printIcon, moreIcon
        },
        data() {
            let moreActions = [
                {
                    title: LanguageService.getValueByKey("preview_itemActions_download"),
                },
                {
                    title: LanguageService.getValueByKey("preview_itemActions_title"),
                }
            ];
            return {moreActions}
        },
        methods: {
            $_back() {
                this.$router.go(-1);
            }
        },
        computed: {
            showActions() {
                return this.$route.name === 'item'
            }
        }
    }
</script>