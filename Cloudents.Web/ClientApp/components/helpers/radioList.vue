<template functional>
    <v-expansion-panel expand>
        <v-expansion-panel-content :value=true hide-actions>
            <template slot="header">
                <div>{{props.title}}</div>
                <div class="header__icon">
                    <v-icon>sbf-arrow-button</v-icon>
                </div>
            </template>
            <div class="sort-filter">
                <v-checkbox v-if="props.values" v-for="s in props.values" @click="props.callback(s)" :label="s" :key="s" hide-details></v-checkbox>
            </div>
        </v-expansion-panel-content>
    </v-expansion-panel>
</template>
<!--<script>
    changeCallback: { type: Function },
    title: { }
</script>-->
<style src="./radioList.less" lang="less"></style>
