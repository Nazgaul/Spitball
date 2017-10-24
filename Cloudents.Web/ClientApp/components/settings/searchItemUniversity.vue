<template>
    <v-list-tile :key="item.id" @click="$_universitySelected(item.id)">
        <v-list-tile-action class="mr-2 pa-2">
            <img class="uni" :src="item.image" alt="">
        </v-list-tile-action>
        <v-list-tile-content>
            <v-list-tile-title v-text="item.name"></v-list-tile-title>
        </v-list-tile-content>
    </v-list-tile>
</template>
<script>
    import { mapActions } from 'vuex'
    export default {
        props: { item: { type: Object, required: true } },
        methods: {
            ...mapActions(['updateUniversity']),
            $_universitySelected(val) {
                this.updateUniversity(val); 
            }
        }
    }
</script>
<style scoped>
    .uni {
    width:2em;
    height:2em;
    }
</style>