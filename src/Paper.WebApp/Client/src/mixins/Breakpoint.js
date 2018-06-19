export default {
  data: () => ({
    clientWidth: 0,
    clientHeight: 0
  }),
  computed: {
    $breakpoint () {
      let xs = this.clientWidth < 600
      let sm = this.clientWidth < 960 && !xs
      let md = this.clientWidth < 1264 && !(sm || xs)
      let lg = this.clientWidth < 1904 && !(md || sm || xs)
      let xl = this.clientWidth >= 1904 && !(lg || md || sm || xs)

      let xsOnly = xs
      let smOnly = sm
      let smAndDown = (xs || sm) && !(md || lg || xl)
      let smAndUp = !xs && (sm || md || lg || xl)
      let mdOnly = md
      let mdAndDown = (xs || sm || md) && !(lg || xl)
      let mdAndUp = !(xs || sm) && (md || lg || xl)
      let lgOnly = lg
      let lgAndDown = (xs || sm || md || lg) && !xl
      let lgAndUp = !(xs || sm || md) && (lg || xl)
      let xlOnly = xl

      let name
      switch (true) {
        case (xs):
          name = 'xs'
          break
        case (sm):
          name = 'sm'
          break
        case (md):
          name = 'md'
          break
        case (lg):
          name = 'lg'
          break
        default:
          name = 'xl'
          break
      }

      let result = {
        'xs': xs,
        'sm': sm,
        'md': md,
        'lg': lg,
        'xl': xl,
        'name': name,
        'xsOnly': xsOnly,
        'smOnly': smOnly,
        'smAndDown': smAndDown,
        'smAndUp': smAndUp,
        'mdOnly': mdOnly,
        'mdAndDown': mdAndDown,
        'mdAndUp': mdAndUp,
        'lgOnly': lgOnly,
        'lgAndDown': lgAndDown,
        'lgAndUp': lgAndUp,
        'xlOnly': xlOnly,
        'width': this.clientWidth,
        'height': this.clientHeight
      }
      return result
    }
  },
  methods: {
    _updateDimensions () {
      this.clientWidth = Math.max(document.documentElement.clientWidth, window.innerWidth || 0)
      this.clientHeight = Math.max(document.documentElement.clientHeight, window.innerHeight || 0)
    }
  },
  mounted () {
    this.$nextTick(() => {
      this._updateDimensions()
      window.addEventListener('resize', this._updateDimensions, {'passive': true})
    })
  },
  destroyed () {
    window.removeEventListener('resize', this._updateDimensions)
  }
}
