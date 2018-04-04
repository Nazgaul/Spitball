<template>
    <div class="actions">
        <v-tooltip bottom v-if="showActions">
            <info-icon slot="activator" class="info-icon ml-4"></info-icon>
            <span>This item might be subject to certain laws and school policies that restrict third-party use of materials prepared by educators. It is your sole responsibility to be aware of and to abide by any such laws or policies that may apply to you. You may access materials solely for your information and your non-commercial, personal use and as intended through the normal functionality of the site and service.</span>
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
                    title: "Download"
                },
                {
                    title: "Print"
                }
            ]
            return { moreActions}
        },
        methods: {
            $_back() {
                this.$router.go(-1);
            }
        },
        computed: {
            showActions() { return this.$route.name==='item'}
        }
    }
</script>