<template>
    <div>
        <div class="title">{{pageData.details.title}}</div>
        <result-book :item="pageData.details" :isDetails="true"></result-book>
        details {{id}}, type {{type}}
        <scroll-list :loadMore="!isEmpty" v-if="pageData.data" @scroll="value => {pageData.data=pageData.data.concat(value) }">
            <v-list>
                <template v-for="(item,index) in pageData.data">
                    <v-list-tile>
                        <v-list-tile-content>
                            <v-list-tile-sub-title>
                                <component :is="'result-'+item.template" :item="item"></component>
                            </v-list-tile-sub-title>                        </v-list-tile-content>
                    </v-list-tile>
                    <v-divider></v-divider>
                </template>    
               
    </v-list>
</scroll-list>
    </div>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins: [pageMixin],
        props: { id: { type: String }, type: {type:String}}
    }
</script>