<template>
    <regular-header v-if="!$vuetify.breakpoint.xsOnly"
                    :currentSelection="myTab"
                    :submitRoute="`/${myTab}`"></regular-header>
</template>

<script>
    import regularHeader from "../../header/header.vue"
    import {mapGetters} from 'vuex'
    export default {
        components: {regularHeader},
        computed:{
            ...mapGetters(['getCurrentVertical']),
            myTab(){
                return this.getCurrentVertical || 'ask'
            }
        }
    }
</script>
