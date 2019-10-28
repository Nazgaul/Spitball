<template>
    <regular-header v-if="!$vuetify.breakpoint.xsOnly"
                    :currentSelection="myTab"
                    :submitRoute="`/${myTab}`"></regular-header>
</template>

<script>
    import regularHeader from "../../header/header.vue"
    export default {
        components: {regularHeader},
        computed:{
            myTab(){
                return 'ask'
            }
        }
    }
</script>
