<template>

                <div>
                    <a target="_blank" :href="item.url" v-for="item in items">
                    <v-card-text>
                        <span v-if="item.name">name:{{item.name}}</span>
                        <span v-if="item.city">city:{{item.city}}</span>
                        <span v-if="item.state">state:{{item.state}}</span>
                        <span v-if="item.fee">fee:{{item.fee}}</span>
                        <span v-if="item.online">online:{{item.online}}</span>
                        <span v-if="item.location">location:{{item.location}}</span>
                        <span v-if="item.termFound">termFound:{{item.termFound}}</span>
                    </v-card-text>
                    <v-card-media :src="item.image" height="125px"
                                  contain></v-card-media>
                    </a>
                </div>
</template>
<script>
    export default {
        computed: {
            items: function () { return this.$store.getters.items }
        }
    }
</script>