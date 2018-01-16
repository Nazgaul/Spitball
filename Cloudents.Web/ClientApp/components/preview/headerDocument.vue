<template>
    <v-toolbar app fixed :height="$vuetify.breakpoint.mdAndUp ? 140 : 182" class="header elevation-0">
        <v-layout column class="header-elements ma-0">
            <main-header class="elevation-1"></main-header>
            <slot name="header">
                <v-toolbar class="item-toolbar elevation-0" height="80">
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
            </slot>

        </v-layout>
    </v-toolbar>
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