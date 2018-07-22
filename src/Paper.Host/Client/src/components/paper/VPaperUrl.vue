<template lang="pug">
  a(@click.stop="goToLink()")
    | {{ field.title }}
</template>

<script>
  export default {
    props: ['field'],

    methods: {
      goToLink () {
        this.$paper.requester.redirectToPage(this.field.value)
      }
    }
  }
</script>
