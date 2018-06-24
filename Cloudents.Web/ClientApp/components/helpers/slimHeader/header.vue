<template>
    <regular-header v-if="hideMobile?!$vuetify.breakpoint.smAndDown:true" ref="mainHeader" :toolbarHeight="$vuetify.breakpoint.smAndDown?105:64"  :currentSelection="myTab"
                    :submitRoute="`/${myTab}`"></regular-header>
</template>

<script>
    import regularHeader from "../header.vue"
    import {mapGetters} from 'vuex'
    export default {
        components: {regularHeader},
        computed:{
            ...mapGetters(['getCurrentVertical']),
            myTab(){
                return this.getCurrentVertical || 'ask'
            }
        },
        props:{hideMobile:{type:Boolean,default:false}}
    }
</script>
