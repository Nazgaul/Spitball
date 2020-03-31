<template>
    <div class="menuListStudent">
        <v-list-item
            v-for="link in studentMenuListFilter"
            :to="{ name: link.url }"
            :key="link.title"
            class="link"
            color="#fff"
            sel="menu_row"
        >
            <v-list-item-action>
                <v-icon class="userMenu_icons">{{link.icon}}</v-icon>
            </v-list-item-action>
            <v-list-item-content>
                <v-list-item-title><span class="userMenu_titles">{{link.title}}</span></v-list-item-title>
            </v-list-item-content>
        </v-list-item>

        <v-list-item sel="menu_row" link :href="satelliteService.getSatelliteUrlByName('faq')" target="_blank">
          <v-list-item-action>
                <v-icon class="userMenu_icons">sbf-help</v-icon>
          </v-list-item-action>
          <v-list-item-content>
                <v-list-item-title class="subheading userMenu_titles">{{$t('menuList_help')}}</v-list-item-title>
          </v-list-item-content>
        </v-list-item>
    </div>
</template>

<script>
import satelliteService from '../../../../services/satelliteService';

export default {
    props: {
        satelliteLinksStudent: {
            required: true
        },
        satelliteLinksTeacher: {
            required: false
        },
    },
    data() {
        return {
            satelliteService
        }
    },
    computed: {
        studentMenuListFilter() {
            let isSold = this.$store.getters.accountUser?.isSold
            return this.satelliteLinksStudent.filter(link => (link.url === 'mySales' && isSold) || (link.url !== 'mySales'))
        }
    }
}
</script>

<style lang="less">
    .menuListStudent {
        .link:hover {
            background: #efefef;
        }
    }
</style>