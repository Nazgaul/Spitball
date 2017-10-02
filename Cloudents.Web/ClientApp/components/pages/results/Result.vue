<template>
    <div class="content">
        <h1 ><span v-if="isEmpty" v-html="page.emptyState"></span>
        <span v-else v-html="page.title"></span>{{dynamicHeader}}</h1>
        <slot name="options">
            <radio-list :values="page.filter" @click="$_changeFilter" model="filter" :value="filterOptions"></radio-list>
            sort: <radio-list :values="page.sort" @click="$_updateSort" model="sort" v-model="sort"></radio-list>
            <radio-list :values="subFilters" @click="$_changeSubFilter" model="subFilter" :value="subFilter"></radio-list>
        </slot>
        <slot></slot>
       <slot name="emptyState" v-show="isEmpty"></slot>
    </div>
</template>
<script>
    import { pageMixin } from './mixins'
    export default {
        mixins:[pageMixin]
    }
</script>