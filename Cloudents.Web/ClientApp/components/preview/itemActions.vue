<template>
    <v-flex>
        <v-tooltip bottom v-if="showActions">
            <info-icon  class="ml-4" slot="activator"></info-icon>
            <span>This item might be subject to certain laws and school policies that restrict third-party use of materials prepared by educators. It is your sole responsibility to be aware of and to abide by any such laws or policies that may apply to you. You may access materials solely for your information and your non-commercial, personal use and as intended through the normal functionality of the site and service.</span>
        </v-tooltip>
        <a target="_blank" :href="$route.path+'download'" class="ml-4" v-if="showActions">
            <download-icon></download-icon>
        </a>
        <a target="_blank" :href="$route.path+'print'" v-if="showActions" class="ml-4 pt-4">
            <print-icon ></print-icon>
        </a>
        <v-menu offset-y v-if="showActions">
            <more-icon class="ml-4" slot="activator"></more-icon>
            <v-list>
                <v-list-tile v-for="item in moreActions" :key="item.title" @click="">
                    <v-list-tile-title>{{ item.title }}</v-list-tile-title>
                </v-list-tile>
            </v-list>
        </v-menu>
        <a href="#" @click.prevent="$_back"> 
            <close-icon class="ml-4"></close-icon>
        </a>
    </v-flex>
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
            let moreActions = [{ title: "rename" }, { title: "Flag an item" }]
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