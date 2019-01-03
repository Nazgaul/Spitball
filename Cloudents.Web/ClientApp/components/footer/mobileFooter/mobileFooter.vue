<template>
    <div>
        <v-bottom-nav
                :active.sync="activeBtn"
                :value="true"
                fixed
                color="white"
        >
            <v-btn flat color="teal">
                <span>Recents</span>
                <v-icon>sbf-icon-earners</v-icon>
            </v-btn>

            <v-btn flat color="teal">
                <span>Favorites</span>
                <v-icon>sbf-icon-promotions</v-icon>
            </v-btn>

            <v-btn flat color="teal" @click="changemobileMarketingBoxState()">
                <span>Nearby</span>
                <v-icon>sbf-icon</v-icon>
            </v-btn>
        </v-bottom-nav>
    </div>
</template>
<script>
    import { mapActions, mapGetters } from 'vuex'
    export default {
        name: "mobileFooter",
        data() {
            return {
                activeBtn: 1,
                showNav: true
            }                
        },
        props: {
            propName: {
                type: Number,
                default: 0
            },
        },
        computed: {

        },
        methods: {
            ...mapActions(['changemobileMarketingBoxState']),
            name() {
                
            }
        },
    }
</script>

<style scoped>

</style>