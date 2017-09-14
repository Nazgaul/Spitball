<template v-once>
    <div>
        <form v-on:submit.prevent="search">
            <input v-model.trim="msg" />
            <input type="submit" value="search" />
        </form>

        <div>{{result}}</div>

        <vertical-collection></vertical-collection>
    </div>
</template>

<script src="./home.js"></script>