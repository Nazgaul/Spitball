<template>
            <main-header class="elevation-0" :toolbarHeight="$vuetify.breakpoint.mdAndUp ? 140 : 182" layoutClass="header-elements ma-0">
                <v-toolbar class="item-toolbar elevation-0 mt-2" height="80" slot="extraHeader">
                    <div class="toolbar-content">
                        <v-layout row align-center justify-space-between>
                            <div class="item-name">{{item.name}}</div>
                            <item-actions></item-actions>
                        </v-layout>
                        <v-flex class="item-meta mt-2">
                            <v-layout row align-center justify-space-between>
                                <div class="author">{{item.owner}}</div>
                                <div class="date">{{item.date | mediumDate}}</div>
                            </v-layout>
                        </v-flex>
                    </div>
                </v-toolbar>
            </main-header>
</template>

<script>
    import itemActions from './itemActions.vue'
    import mainHeader from '../helpers/header.vue'
    import { mapGetters } from 'vuex'
    export default {
        components: {
            mainHeader, itemActions
        },
        computed: { ...mapGetters({ 'item': 'itemDetails' }) },
        filters: {
            mediumDate: function (value) {
                if (!value) return '';
                let date = new Date(value);
                return date.toLocaleString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
            }
        }
    }
</script>
<style src="./headerDocument.less" lang="less"></style>